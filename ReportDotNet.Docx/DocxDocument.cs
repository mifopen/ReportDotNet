using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using Table = ReportDotNet.Core.Table;

namespace ReportDotNet.Docx
{
	internal class DocxDocument: IDocument
	{
		private readonly WordprocessingDocument document;
		private readonly MemoryStream memoryStream;
		private readonly List<Page> pages = new List<Page>();
		private PageLayout defaultPageLayout;
		private Table defaultFooter;

		public DocxDocument()
		{
			memoryStream = new MemoryStream();
			document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
			document.AddMainDocumentPart();
			document.MainDocumentPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document { Body = new Body() };
			var stylesPart = document.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
			DefaultStyles.Styles.Value.Save(stylesPart);
			//todo looks ugly
			SetDefaultPageLayout(new PageLayout
								 {
									 Orientation = PageOrientation.Portrait
								 });
		}

		public byte[] Save()
		{
			foreach (var page in pages)
			{
				var isLastPage = page == pages.Last();
				document.MainDocumentPart.Document.Body.Append(page.Convert(this, document, isLastPage));
			}

			using (memoryStream)
				using (document)
					document.Close();
			return memoryStream.ToArray();
		}

		public IDocument AddPage(Page page)
		{
			pages.Add(page);
			return this;
		}

		public void SetDefaultPageLayout(PageLayout pageLayout)
		{
			defaultPageLayout = pageLayout;
		}

		PageLayout IDocument.GetDefaultPageLayout()
		{
			return defaultPageLayout;
		}

		public void SetDefaultFooter(Table table)
		{
			defaultFooter = table;
			
		}

		public Table GetDefaultFooter()
		{
			return defaultFooter;
		}
	}
}