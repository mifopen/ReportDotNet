using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using ReportDotNet.Playground.Template;
using ReportDotNet.Web.App;

namespace ReportDotNet.Web.Controllers
{
    public class HomeController: Controller
    {
        private readonly WordToPdfConverter wordToPdfConverter;
        private readonly PdfToPngConverter pdfToPngConverter;
        private readonly ReportRenderer reportRenderer;
        private readonly DirectoryWatcher directoryWatcher;


        public HomeController(WordToPdfConverter wordToPdfConverter,
                              PdfToPngConverter pdfToPngConverter,
                              ReportRenderer reportRenderer,
                              DirectoryWatcher directoryWatcher)
        {
            this.wordToPdfConverter = wordToPdfConverter;
            this.pdfToPngConverter = pdfToPngConverter;
            this.reportRenderer = reportRenderer;
            this.directoryWatcher = directoryWatcher;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Render()
        {
            var templateDirectoryPath = GetTemplateDirectoryPath();
            var renderedReport = reportRenderer.Render(templateDirectoryPath);
            var pdf = wordToPdfConverter.Convert(renderedReport.Bytes);
            CachedImages = pdfToPngConverter.Convert(pdf);
            return Json(new
                        {
                            Log = string.Join("<br/>", renderedReport.Log),
                            PagesCount = CachedImages.Length
                        }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileContentResult GetPage(int pageNumber)
        {
            return File(CachedImages[pageNumber], "image/png");
        }

        [HttpGet]
        public FileContentResult GetDocx()
        {
            var templateDirectoryPath = GetTemplateDirectoryPath();
            var renderedReport = reportRenderer.Render(templateDirectoryPath);
            return File(renderedReport.Bytes,
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        "result.docx");
        }

        private string GetTemplateDirectoryPath()
        {
            var templateProjectDirectory = GetTemplateProjectDirectory();
            var templateDirectoryName = System.IO.File.ReadAllText(Path.Combine(templateProjectDirectory, "CurrentTemplateDirectory.txt"));
            var templateDirectoryPath = Path.Combine(templateProjectDirectory, templateDirectoryName);
            EnsureTemplateDirectory(templateDirectoryPath, templateDirectoryName, templateProjectDirectory);
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

        private static string GetTemplateProjectDirectory()
        {
            var webProjectPath = HostingEnvironment.ApplicationPhysicalPath;
            var solutionPath = Path.Combine(webProjectPath, "..");
            return Path.Combine(solutionPath, typeof(StubForNamespace).Namespace);
        }

        private const string cacheKey = "HomeController.CachedImages";

        private byte[][] CachedImages
        {
            get => (byte[][]) ControllerContext.HttpContext.Cache[cacheKey];
            set => ControllerContext.HttpContext.Cache[cacheKey] = value;
        }
    }
}