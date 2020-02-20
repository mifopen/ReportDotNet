using System;
using System.Collections.Generic;

namespace ReportDotNet.Core
{
    public class TableBuilder
    {
        private readonly TableParameters parameters = new TableParameters();

        public TableBuilder Borders(Borders borders) => Chain(p => p.Borders = borders);
        public TableBuilder FontSize(int fontSize) => Chain(p => p.FontSize = fontSize);

        public TableBuilder CellMargin(int? left = null,
                                       int? right = null,
                                       int? top = null,
                                       int? bottom = null) => Chain(p =>
                                                                   {
                                                                       p.CellMarginLeft = left;
                                                                       p.CellMarginRight = right;
                                                                       p.CellMarginTop = top;
                                                                       p.CellMarginBottom = bottom;
                                                                   });

        public TableBuilder Width(int width) => Chain(p => p.Width = width);
        public TableBuilder Add(params Row[] rows) => Chain(p => p.Rows.AddRange(rows));
        public TableBuilder Add(IEnumerable<Row> rows) => Chain(p => p.Rows.AddRange(rows));

        public Table Build() => new Table(parameters);

        public static implicit operator Table(TableBuilder builder) => builder.Build();

        private TableBuilder Chain(Action<TableParameters> action)
        {
            action(parameters);
            return this;
        }
    }
}