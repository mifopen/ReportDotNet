using System;

namespace ReportDotNet.Core
{
	public class TableBuilder
	{
		private readonly TableParameters parameters = new TableParameters();

		public TableBuilder Borders(Borders borders) => Chain(p => p.Borders = borders);
		public TableBuilder FontSize(int fontSize) => Chain(p => p.FontSize = fontSize);
		public TableBuilder CellMarginLeft(int cellMarginLeft) => Chain(p => p.CellMarginLeft = cellMarginLeft);
		public TableBuilder CellMarginRight(int cellMarginRight) => Chain(p => p.CellMarginRight = cellMarginRight);
		public TableBuilder Width(int width) => Chain(p => p.Width = width);
		public TableBuilder Rows(params Row[] rows) => Chain(p => p.Rows = rows);

		public Table Build() => new Table(parameters);

		public static implicit operator Table(TableBuilder builder) => builder.Build();

		private TableBuilder Chain(Action<TableParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}