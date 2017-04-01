using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReportDotNet.Core;
using ReportDotNet.Playground.Template;

namespace ReportDotNet.Web.App
{
	public class ReportRenderer
	{
		public Report Render(IDocument document, string sourcePath)
		{
			var sourceAssembly = CreateAssembly(sourcePath);
			var sourceType = sourceAssembly.GetTypes().Single(x => !x.IsNested);
			var log = new List<string>();
			Action<int, string, object> logAction = (lineNumber, line, obj) => log.Add($"#{lineNumber}: {line}: {obj}");
			GetFillDocumentMethod(sourceType).Invoke(null, new object[] { document, logAction });
			return new Report
				   {
					   Log = log.ToArray(),
					   RenderedBytes = document.Save()
				   };
		}

		private static MethodInfo GetFillDocumentMethod(Type type)
		{
			return type.GetMethods()
					   .Single(m =>
							   {
								   var parameters = m.GetParameters();
								   return m.IsStatic
										  && parameters.Length == 2
										  && parameters[0].ParameterType == typeof(IDocument)
										  && parameters[1].ParameterType == typeof(Action<int, string, object>);
							   });
		}

		private static Assembly CreateAssembly(string sourcePath)
		{
			var references = new[]
							 {
								 Assembly.GetExecutingAssembly(),
								 typeof(Template).Assembly
							 }
				.SelectMany(x => x.GetReferencedAssemblies())
				.Select(x => x.FullName)
				.Distinct()
				.Select(Assembly.Load)
				.Select(a => MetadataReference.CreateFromFile(a.Location))
				.ToArray();
			var compilation = CSharpCompilation.Create(assemblyName: "NewReport.dll",
													   syntaxTrees: new[] { GetSyntaxTree(sourcePath) },
													   references: references,
													   options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			try
			{
				using (var ms = new MemoryStream())
				{
					var result = compilation.Emit(ms);
					if (result.Success)
						return Assembly.Load(ms.ToArray());

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

		private static readonly Regex logRegex = new Regex("\\slog[(](.*)[)];", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly Regex logParameterRegex = new Regex("\\sAction<object> log", RegexOptions.Compiled | RegexOptions.Singleline);

		private static SyntaxTree GetSyntaxTree(string fileName)
		{
			var lines = File.ReadAllLines(fileName)
							.Select((l, i) => logParameterRegex.Replace(l, "Action<int, string, object> log"))
							.Select((l, i) => logRegex.Replace(l, $"log({i + 1}, \"$1\", $1);"))
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