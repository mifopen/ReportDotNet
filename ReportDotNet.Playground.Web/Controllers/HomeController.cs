using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using ReportDotNet.Core;
using ReportDotNet.Docx;
using ReportDotNet.Playground.Template;
using ReportDotNet.WebApp.App;

namespace ReportDotNet.WebApp.Controllers
{
	public class HomeController: Controller
	{
		private readonly WordToPdfConverter wordToPdfConverter;
		private readonly PdfToPngConverter pdfToPngConverter;
		private readonly ReportRenderer reportRenderer;
		private readonly ReportWatcher reportWatcher;


		public HomeController(WordToPdfConverter wordToPdfConverter,
							  PdfToPngConverter pdfToPngConverter,
							  ReportRenderer reportRenderer,
							  ReportWatcher reportWatcher)
		{
			this.wordToPdfConverter = wordToPdfConverter;
			this.pdfToPngConverter = pdfToPngConverter;
			this.reportRenderer = reportRenderer;
			this.reportWatcher = reportWatcher;
		}

		public ActionResult Index()
		{
			var templatePath = GetTemplatePath();
			reportWatcher.Watch(templatePath);
			return View(model: templatePath);
		}

		[HttpGet]
		public JsonResult Render()
		{
			var document = Create.Document.Docx();
			var reportData = reportRenderer.Render(document, GetTemplatePath());
			var pdf = wordToPdfConverter.Convert(reportData.RenderedBytes);
			CachedImages = pdfToPngConverter.Convert(pdf);
			return Json(new
						{
							Log = string.Join("<br>", reportData.Log),
							PagesCount = CachedImages.Length
						}, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public FileContentResult GetPage(int pageNumber)
		{
			return File(CachedImages[pageNumber], "image/png");
		}

		private static string GetTemplatePath()
		{
			var webProjectPath = HostingEnvironment.ApplicationPhysicalPath;
			var solutionPath = Path.Combine(webProjectPath, "..");
			var templateProjectPath = Path.Combine(solutionPath, typeof(Template).Namespace);
			return Path.Combine(templateProjectPath, "Template.cs");
		}

		private const string cacheKey = "HomeController.CachedImages";

		private byte[][] CachedImages
		{
			get { return (byte[][]) ControllerContext.HttpContext.Cache[cacheKey]; }
			set { ControllerContext.HttpContext.Cache[cacheKey] = value; }
		}
	}
}