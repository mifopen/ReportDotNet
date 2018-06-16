using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ReportDotNet.Web.App;

namespace ReportDotNet.Web.Controllers
{
    public class HomeController: Controller
    {
        private readonly WordToPdfConverter wordToPdfConverter;
        private readonly PdfToPngConverter pdfToPngConverter;
        private readonly ReportRenderer reportRenderer;
        private readonly DirectoryWatcher directoryWatcher;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IMemoryCache memoryCache;

        public HomeController(WordToPdfConverter wordToPdfConverter,
                              PdfToPngConverter pdfToPngConverter,
                              ReportRenderer reportRenderer,
                              DirectoryWatcher directoryWatcher,
                              IHostingEnvironment hostingEnvironment,
                              IMemoryCache memoryCache)
        {
            this.wordToPdfConverter = wordToPdfConverter;
            this.pdfToPngConverter = pdfToPngConverter;
            this.reportRenderer = reportRenderer;
            this.directoryWatcher = directoryWatcher;
            this.hostingEnvironment = hostingEnvironment;
            this.memoryCache = memoryCache;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Render(string templateName)
        {
            var templateDirectoryPath = GetTemplateDirectoryPath(templateName);
            var renderedReport = reportRenderer.Render(templateDirectoryPath);
            var pdf = wordToPdfConverter.Convert(renderedReport.Bytes);
            CachedImages = pdfToPngConverter.Convert(pdf);
            return Json(new
                        {
                            Log = string.Join("<br/>", renderedReport.Log),
                            PagesCount = CachedImages.Length
                        });
        }

        [HttpGet]
        public FileContentResult GetPage(int pageNumber)
        {
            return File(CachedImages[pageNumber], "image/png");
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
                                  .Except(new[] { "bin", "obj", "Properties" });
                throw new Exception($"Are you sure that directory {templateDirectoryName} exists in template project?" +
                                    $" There are only {string.Join(", ", directories)}.");
            }
        }

        private string GetTemplateProjectDirectory()
        {
            var webProjectPath = hostingEnvironment.ContentRootPath;
            return Path.Combine(webProjectPath, "Examples");
        }

        private const string cacheKey = "HomeController.CachedImages";

        private byte[][] CachedImages
        {
            get => memoryCache.Get<byte[][]>(cacheKey);
            set => memoryCache.Set(cacheKey, value);
        }
    }
}