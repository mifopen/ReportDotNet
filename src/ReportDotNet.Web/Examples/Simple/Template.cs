using System;
using System.Drawing;
using ReportDotNet.Core;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Web.Examples.Simple
{
    public static class Template
    {
        public static void FillDocument(IDocument document,
                                        Action<object> log)
        {
            var footer = Table(300)
                .Add(Row(Cell()
                             .Add(Paragraph()
                                      .Add(Field.PageNumber)
                                      .Add(" / ")
                                      .Add(Field.PageCount))));
            document.AddPage(Page()
                                 .Orientation(PageOrientation.Portrait)
                                 .Footer(footer)
                                 .Add(Paragraph("This is the first paragraph on the first page"),
                                      Paragraph("\\n in text will add new line \n just like that"),
                                      Paragraph("Text with right alignment")
                                          .Alignment(Alignment.Right),
                                      Paragraph("Bold 14pt with background color")
                                          .Bold()
                                          .BackgroundColor(Color.Aqua)
                                          .FontSize(14),
                                      Paragraph("Let's add another page and play with tables")));

            var page = Page()
                .Orientation(PageOrientation.Landscape)
                .Footer(footer)
                .Add(Table(400)
                         .Borders(Borders.All)
                         .Add(Row(Cell("1,1", 200)
                                      .Alignment(Alignment.Right),
                                  Cell("1,2", 200)
                                      .BorderSize(3)),
                              Row(Cell("2,1", 200)
                                      .BackgroundColor(Color.Aqua),
                                  Cell("2,2", 100)
                                      .MergeDown()
                                      .Alignment(Alignment.Center)
                                      .VerticalAlignment(VerticalAlignment.Center)
                                      .TextDirection(TextDirection.RightLeft_TopBottom)),
                              Row(Cell("3,1", 200),
                                  Cell("3,2", 100)
                                      .MergeUp())));

            page.Add(Paragraph());

            var table = Table(700)
                .Borders(Borders.Top | Borders.Bottom);
            for (var i = 16; i < 100; i += 10)
                table.Add(Row(i)
                              .HeightType(RowHeightType.Exact)
                              .Add(Cell($"row with height = {i}")
                                       .VerticalAlignment(VerticalAlignment.Center)));

            page.Add(table);

            page.Add(Paragraph()
                         .Add(StubPicture()
                                  .MaxWidth(200)
                                  .MaxHeight(100)
                                  .Color(Color.Brown)));

            document.AddPage(page);

            var variableName = "Some log text";
            log(variableName);
        }
    }
}