using System.Collections.Generic;

namespace ReportDotNet.Core
{
	public class TableParameters
	{
		public int Width { get; set; }
		public Borders? Borders { get; set; }
		public int? CellMarginLeft { get; set; }
		public int? CellMarginRight { get; set; }
		public int? FontSize { get; set; }
		public IEnumerable<Row> Rows { get; set; } = new Row[0];
	}
}