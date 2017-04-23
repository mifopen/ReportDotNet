namespace ReportDotNet.Core
{
	public static class Factories
	{
		public static PageBuilder Page() => new PageBuilder();

		public static TableBuilder Table(params Row[] rows) => new TableBuilder().Add(rows);
		public static TableBuilder Table(int width) => new TableBuilder().Width(width);

		public static RowBuilder Row() => new RowBuilder();
		public static RowBuilder Row(int height) => new RowBuilder().Height(height);
		public static RowBuilder Row(params Cell[] cells) => new RowBuilder().Add(cells);

		public static CellBuilder Cell() => new CellBuilder();
		public static CellBuilder Cell(int? width) => new CellBuilder().Width(width);
		public static CellBuilder Cell(string text, int? width = null) => new CellBuilder().Add(text).Width(width);

		public static ParagraphBuilder Paragraph() => new ParagraphBuilder();
		public static ParagraphBuilder Paragraph(string text) => new ParagraphBuilder().Add(text);

		public static PictureBuilder Picture(byte[] bytes) => new PictureBuilder().Bytes(bytes);
		public static StubPictureBuilder StubPicture() => new StubPictureBuilder();
	}
}