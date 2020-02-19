using System.Collections.Generic;
using System.Linq;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Core
{
	public class RowParameters
	{
		public int? Height { get; set; }
		public RowHeightType HeightType { get; set; }
		private readonly List<Cell> cells = new List<Cell>();

		public void AddCells(IEnumerable<Cell> cell) => cells.AddRange(cell);

		public Cell[] GetCells()
		{
			return cells.Any() ? cells.ToArray() : new[] { Cell().Build() };
		}
	}
}