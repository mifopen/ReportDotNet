using System.Collections.Generic;

namespace ReportDotNet.Core
{
    public class RowParameters
    {
        public int? Height { get; set; }
        public RowHeightType HeightType { get; set; }
        public List<Cell> Cells { get; } = new List<Cell>();
    }
}