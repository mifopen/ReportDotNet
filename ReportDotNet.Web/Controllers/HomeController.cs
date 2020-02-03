using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ReportDotNet.Web.App;

namespace ReportDotNet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly GoogleDocsUploader googleDocsUploader;
        private readonly ReportRenderer reportRenderer;
        private readonly DirectoryWatcher directoryWatcher;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(GoogleDocsUploader googleDocsUploader,
                              ReportRenderer reportRenderer,
                              DirectoryWatcher directoryWatcher,
                              IWebHostEnvironment webHostEnvironment)
        {
            this.googleDocsUploader = googleDocsUploader;
            this.reportRenderer = reportRenderer;
            this.directoryWatcher = directoryWatcher;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<JsonResult> Render(string templateName)
        {
            var templateDirectoryPath = GetTemplateDirectoryPath(templateName);
            var renderedReport = reportRenderer.Render(templateDirectoryPath);
            var googleDocFileId = await googleDocsUploader.Update(renderedReport.Bytes);
            return Json(new
                        {
                            Log = string.Join("<br/>", renderedReport.Log),
                            GoogleDocUrl = $"https://docs.google.com/document/d/{googleDocFileId}/edit"
                        });
        }

        [HttpGet]
        public FileContentResult GetDocx(string templateName)
        {
            var templateDirectoryPath = GetTemplateDirectoryPath(templateName);
            var renderedReport = reportRenderer.Render(templateDirectoryPath);
            return File(renderedReport.Bytes,
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        "result.docx");
        }

        private string GetTemplateDirectoryPath(string templateName)
        {
            var templateProjectDirectory = GetTemplateProjectDirectory();
            var templateDirectoryPath = Path.Combine(templateProjectDirectory, templateName);
            EnsureTemplateDirectory(templateDirectoryPath, templateName, templateProjectDirectory);
            directoryWatcher.Watch(templateProjectDirectory);
            return templateDirectoryPath;
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
                                  .Except(new[] {"bin", "obj", "Properties"});
                throw new Exception($"Are you sure that directory {templateDirectoryName} exists in template project?" +
                                    $" There are only {string.Join(", ", directories)}.");
            }
        }

        private string GetTemplateProjectDirectory()
        {
            var webProjectPath = webHostEnvironment.ContentRootPath;
            return Path.Combine(webProjectPath, "Examples");
        }
    }
}