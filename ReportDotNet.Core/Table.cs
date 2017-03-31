namespace ReportDotNet.Core
{
	public class Table: IPageElement
	{
		public TableParameters Parameters { get; }

		public Table(TableParameters parameters)
		{
			Parameters = parameters;
			foreach (var row in Parameters.Rows)
				row.Table = this;
		}
	}
}