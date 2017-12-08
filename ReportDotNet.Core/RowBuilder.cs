using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportDotNet.Core
{
    public class RowBuilder
    {
        private readonly RowParameters parameters = new RowParameters();

        public RowBuilder Height(int height) => Chain(p => p.Height = height);
        public RowBuilder HeightType(RowHeightType heightType) => Chain(p => p.HeightType = heightType);

        public RowBuilder Add(params Cell[] cells) =>
            Chain(p => p.AddCells(cells.Where(x => x != null)));

        public RowBuilder Add(IEnumerable<Cell> cells) =>
            Chain(p => p.AddCells(cells.Where(x => x != null)));

        public Row Build() => new Row(parameters);

        public static implicit operator Row(RowBuilder builder) => builder.Build();

        private RowBuilder Chain(Action<RowParameters> action)
        {
            action(parameters);
            return this;
        }
    }
}