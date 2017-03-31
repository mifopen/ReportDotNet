using System;

namespace ReportDotNet.Core
{
	public class RowBuilder
	{
		private readonly RowParameters parameters = new RowParameters();

		public RowBuilder Height(int height) => Chain(p => p.Height = height);
		public RowBuilder HeightType(RowHeightType heightType) => Chain(p => p.HeightType = heightType);
		public RowBuilder Cells(params Cell[] cells) => Chain(p => p.Cells = cells);

		public Row Build() => new Row(parameters);

		public static implicit operator Row(RowBuilder builder) => builder.Build();

		private RowBuilder Chain(Action<RowParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}