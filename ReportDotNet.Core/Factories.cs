namespace ReportDotNet.Core
{
	public static class Factories
	{
		public static PageBuilder Page() => new PageBuilder();

		public static TableBuilder Table(params Row[] rows) => new TableBuilder().Rows(rows);
		public static TableBuilder Table(int width) => new TableBuilder().Width(width);

		public static RowBuilder Row() => new RowBuilder();
		public static RowBuilder Row(params Cell[] cells) => new RowBuilder().Cells(cells);

		public static CellBuilder Cell() => new CellBuilder();
		public static CellBuilder Cell(string text) => new CellBuilder().Text(text);
		public static CellBuilder Cell(int width) => new CellBuilder().Width(width);
		public static CellBuilder Cell(string text, int width) => new CellBuilder().Text(text).Width(width);

		public static ParagraphBuilder Paragraph() => new ParagraphBuilder();
		public static ParagraphBuilder Paragraph(string text) => new ParagraphBuilder().AddText(text);
	}
}