using System;
using System.Linq;

namespace ReportDotNet.Core
{
    public class Row
    {
        public RowParameters Parameters { get; }
        public Table Table { private get; set; }

        public Row(RowParameters parameters)
        {
            Parameters = parameters;
        }

        public int[] GetWidths()
        {
            if (Parameters.Cells.Count(c => c.Parameters.Width == null) > 1)
                throw new InvalidOperationException("Multiple cells with relative width in one row aren't supported");
            var relativeCellWidth = Table.Parameters.Width - Parameters.Cells.Sum(c => c.Parameters.Width ?? 0);
            return Parameters.Cells.Select(x => x.Parameters.Width ?? relativeCellWidth).ToArray();
        }
    }
}