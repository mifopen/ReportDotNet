using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReportDotNet.Core;
using ReportDotNet.Docx;
using ReportDotNet.Playground.Template;

namespace ReportDotNet.Web.App
{
    public class ReportRenderer
    {
        private readonly DirectoryWatcher directoryWatcher;

        public ReportRenderer(DirectoryWatcher directoryWatcher)
        {
            this.directoryWatcher = directoryWatcher;
        }

        public Report Render()
        {
            var templateProjectDirectory = GetTemplateProjectDirectory();
            var templateDirectoryName = File.ReadAllText(Path.Combine(templateProjectDirectory, "CurrentTemplateDirectory.txt"));
            var templateDirectoryPath = Path.Combine(templateProjectDirectory, templateDirectoryName);
            EnsureTemplateDirectory(templateDirectoryPath, templateDirectoryName, templateProjectDirectory);
            directoryWatcher.Watch(templateProjectDirectory);

            return File.Exists(Path.Combine(templateDirectoryPath, "Template.cs"))
                       ? RenderReportDotNetTemplate(templateDirectoryPath)
                       : RenderUnzipDocx(templateDirectoryPath);
        }

        private static Report RenderReportDotNetTemplate(string templateDirectoryPath)
        {
            var templatePath = Path.Combine(templateDirectoryPath, "Template.cs");
            var templateType = CreateTemplateType(templatePath);
            var log = new List<string>();
            Action<int, string, object> logAction = (lineNumber,
                                                     line,
                                                     obj) => log.Add($"#{lineNumber}: {line}: {obj}");
            var method = GetFillDocumentMethod(templateType);
            var document = Create.Document.Docx();
            var arguments = method.GetParameters().Length == 2
                                ? new object[] { document, logAction }
                                : new object[] { document, logAction, templateDirectoryPath };
            method.Invoke(null, arguments);

            return new Report
                   {
                       Log = log.ToArray(),
                       RenderedBytes = document.Save()
                   };
        }

        private static Report RenderUnzipDocx(string templateDirectoryPath)
        {
            const string zipFileName = "somefile.docx";
            File.Delete(zipFileName);
            ZipFile.CreateFromDirectory(templateDirectoryPath, zipFileName);
            var zipBytes = File.ReadAllBytes(zipFileName);
            File.Delete(zipFileName);
            return new Report
                   {
                       Log = new string[0],
                       RenderedBytes = zipBytes
                   };
        }

        private static void EnsureTemplateDirectory(string templateDirectoryPath,
                                                    string templateDirectoryName,
                                                    string templateProjectDirectory)
        {
            if (!Directory.Exists(templateDirectoryPath))
            {
                var directories = new DirectoryInfo(templateProjectDirectory)
                    .EnumerateDirectories()
                    .Select(x => x.Name)
                    .Except(new[] { "bin", "obj", "Properties" });
                throw new Exception($"Are you sure that directory {templateDirectoryName} exists in template project?" +
                                    $" There are only {string.Join(", ", directories)}.");
            }
        }

        private static Type CreateTemplateType(string templatePath)
        {
            var templateAssembly = typeof(StubForNamespace).Assembly;
            var references = templateAssembly.GetReferencedAssemblies()
                                             .Select(x => x.FullName)
                                             .Distinct()
                                             .Select(Assembly.Load)
                                             .Concat(new[] { templateAssembly })
                                             .Select(a => MetadataReference.CreateFromFile(a.Location))
                                             .ToArray();
            var compilation = CSharpCompilation.Create(assemblyName: "NewReport.dll",
                                                       syntaxTrees: new[] { GetSyntaxTree(templatePath) },
                                                       references: references,
                                                       options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            try
            {
                using (var ms = new MemoryStream())
                {
                    var result = compilation.Emit(ms);
                    if (result.Success)
                    {
                        var types = Assembly.Load(ms.ToArray()).GetTypes().Where(x => x.Name == "Template").ToArray();
                        if (types.Length != 1)
                            throw new Exception($"There must be at least one type with name Template in example {templatePath}");

                        return types.Single();
                    }

                    var failures = result.Diagnostics.Where(diagnostic =>
                                                                diagnostic.IsWarningAsError ||
                                                                diagnostic.Severity == DiagnosticSeverity.Error);
                    throw new InvalidOperationException(string.Join(Environment.NewLine, failures.Select(x => $"{x.Id}: {x.GetMessage()}")));
                }
            }
            finally
            {
                File.Delete(compilation.AssemblyName);
            }
        }

        private static string GetTemplateProjectDirectory()
        {
            var webProjectPath = HostingEnvironment.ApplicationPhysicalPath;
            var solutionPath = Path.Combine(webProjectPath, "..");
            return Path.Combine(solutionPath, typeof(StubForNamespace).Namespace);
        }

        private static MethodInfo GetFillDocumentMethod(Type type)
        {
            return type.GetMethods()
                       .Single(m =>
                               {
                                   var parameters = m.GetParameters();
                                   return m.IsStatic
                                          && parameters.Length >= 2
                                          && parameters[0].ParameterType == typeof(IDocument)
                                          && parameters[1].ParameterType == typeof(Action<int, string, object>)
                                          && (parameters.Length == 2 || parameters[2].ParameterType == typeof(string));
                               });
        }

        private static readonly Regex logRegex = new Regex("\\slog[(](.*)[)];", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex logParameterRegex = new Regex("\\sAction<object> log", RegexOptions.Compiled | RegexOptions.Singleline);

        private static SyntaxTree GetSyntaxTree(string fileName)
        {
            var lines = File.ReadAllLines(fileName)
                            .Select((l,
                                     i) => logParameterRegex.Replace(l, "Action<int, string, object> log"))
                            .Select((l,
                                     i) => logRegex.Replace(l, $"log({i + 1}, \"$1\", $1);"))
                            .ToArray();
            return CSharpSyntaxTree.ParseText(string.Join(Environment.NewLine, lines));
        }

        public class Report
        {
            public byte[] RenderedBytes { get; set; }
            public string[] Log { get; set; }
        }
    }
}