using System;
using System.Web.Mvc;
using ReportDotNet.Core;
using ReportDotNet.Docx;
using ReportDotNet.Web.App;

namespace ReportDotNet.Web.Controllers
{
    public class HomeController: Controller
    {
        private readonly WordToPdfConverter wordToPdfConverter;
        private readonly PdfToPngConverter pdfToPngConverter;
        private readonly ReportRenderer reportRenderer;


        public HomeController(WordToPdfConverter wordToPdfConverter,
                              PdfToPngConverter pdfToPngConverter,
                              ReportRenderer reportRenderer)
        {
            this.wordToPdfConverter = wordToPdfConverter;
            this.pdfToPngConverter = pdfToPngConverter;
            this.reportRenderer = reportRenderer;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Render()
        {
            try
            {
                var document = Create.Document.Docx();
                var reportData = reportRenderer.Render(document);
                var pdf = wordToPdfConverter.Convert(reportData.RenderedBytes);
                CachedImages = pdfToPngConverter.Convert(pdf);
                return Json(new
                            {
                                Log = string.Join("<br>", reportData.Log),
                                PagesCount = CachedImages.Length
                            }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet]
        public FileContentResult GetPage(int pageNumber)
        {
            try
            {
                return File(CachedImages[pageNumber], "image/png");
            }
            catch (Exception e)
            {
                throw;
            }
        }


        private const string cacheKey = "HomeController.CachedImages";

        private byte[][] CachedImages
        {
            get { return (byte[][]) ControllerContext.HttpContext.Cache[cacheKey]; }
            set { ControllerContext.HttpContext.Cache[cacheKey] = value; }
        }
    }
}