using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using PdfiumViewer;

namespace ReportDotNet.WebApp.App
{
	public class PdfToPngConverter
	{
		public byte[][] Convert(byte[] source)
		{
			using (var document = PdfDocument.Load(new MemoryStream(source)))
				return Enumerable.Range(0, document.PageCount)
								 .Select(page => RenderPage(document, page))
								 .ToArray();
		}

		private static byte[] RenderPage(PdfDocument pdf, int page)
		{
			const int multiplier = 3;
			using (var pageImage = pdf.Render(page, (int) pdf.PageSizes[page].Width*multiplier, (int) pdf.PageSizes[page].Height*multiplier, 256, 256, PdfRenderFlags.None))
			{
				var ms = new MemoryStream();
				pageImage.Save(ms, ImageFormat.Png);
				return ms.ToArray();
			}
		}
	}
}