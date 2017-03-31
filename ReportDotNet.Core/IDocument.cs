namespace ReportDotNet.Core
{
	public interface IDocument
	{
		IDocument AddPage(Page page);
		byte[] Save();
		void SetDefaultPageLayout(PageLayout pageLayout);
		PageLayout GetDefaultPageLayout();
		void SetDefaultFooter(Table table);
		Table GetDefaultFooter();
	}
}