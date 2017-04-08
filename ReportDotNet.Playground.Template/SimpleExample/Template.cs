using System;
using System.Drawing;
using ReportDotNet.Core;

namespace ReportDotNet.Playground.Template.SimpleExample
{
	public static class Template
	{
		public static void FillDocument(IDocument document, Action<object> log)
		{
			var footer = Factories.Table(300)
				.Add(Factories.Row(Factories.Cell()
							 .Add(Factories.Paragraph()
									  .Add(Field.PageNumber)
									  .Add(" / ")
									  .Add(Field.PageCount))));
			document.AddPage(Factories.Page()
								 .Orientation(PageOrientation.Portrait)
								 .Footer(footer)
								 .Add(Factories.Paragraph("This is the first paragraph on the first page"),
									  Factories.Paragraph("\\n in text will add new line \n just like that"),
									  Factories.Paragraph("Text with right alignment")
										  .Alignment(Alignment.Right),
									  Factories.Paragraph("Bold 14pt")
										  .Bold()
										  .FontSize(14),
									  Factories.Paragraph("Let's add another page and play with tables")));

			var page = Factories.Page()
				.Orientation(PageOrientation.Landscape)
				.Footer(footer)
				.Add(Factories.Table(400)
						 .Borders(Borders.All)
						 .Add(Factories.Row(Factories.Cell("1,1", 200)
									  .Alignment(Alignment.Right),
								  Factories.Cell("1,2", 200)
									  .BorderSize(3)),
							  Factories.Row(Factories.Cell("2,1", 200)
									  .BackgroundColor(Color.Aqua),
								  Factories.Cell("2,2", 100)
									  .MergeDown()
									  .Alignment(Alignment.Center)
									  .VerticalAlignment(VerticalAlignment.Center)
									  .TextDirection(TextDirection.RightLeft_TopBottom)),
							  Factories.Row(Factories.Cell("3,1", 200),
								  Factories.Cell("3,2", 100)
									  .MergeUp())));

			page.Add(Factories.Paragraph());

			var table = Factories.Table(700)
				.Borders(Borders.Top | Borders.Bottom);
			for (var i = 16; i < 100; i += 10)
				table.Add(Factories.Row(i)
							  .HeightType(RowHeightType.Exact)
							  .Add(Factories.Cell($"row with height = {i}")
									   .VerticalAlignment(VerticalAlignment.Center)));

			page.Add(table);

			page.Add(Factories.Paragraph().Add(new TestPicture(200, 100, Color.Brown)));

			document.AddPage(page);

			var variableName = "Some log text";
			log(variableName);
		}
	}
}