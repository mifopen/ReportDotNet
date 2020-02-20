namespace ReportDotNet.Core
{
    public class PageSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public static readonly PageSize A4 = new PageSize { Width = 210, Height = 297 };
    }
}