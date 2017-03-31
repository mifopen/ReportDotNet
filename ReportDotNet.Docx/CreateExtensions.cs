using ReportDotNet.Core;

namespace ReportDotNet.Docx
{
	public static class CreateExtensions
	{
		public static IDocument Docx(this Create _) => new DocxDocument();
	}
}