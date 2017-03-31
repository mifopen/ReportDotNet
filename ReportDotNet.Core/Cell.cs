namespace ReportDotNet.Core
{
	public class Cell
	{
		public CellParameters Parameters { get; }

		public Cell(CellParameters parameters)
		{
			Parameters = parameters;
		}
	}
}