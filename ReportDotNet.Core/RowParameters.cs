using System.Collections.Generic;

namespace ReportDotNet.Core
{
	public class RowParameters
	{
		public int? Height { get; set; }
		public RowHeightType HeightType { get; set; }
		public IEnumerable<Cell> Cells { get; set; } = new Cell[0];
	}
}