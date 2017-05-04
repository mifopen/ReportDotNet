namespace ReportDotNet.Core
{
    public class Page
    {
        public PageParameters Parameters { get; }

        public Page(PageParameters parameters)
        {
            Parameters = parameters;
        }
    }
}