using System.Collections.Generic;

namespace ReportDotNet.Core
{
    public class PageParameters
    {
        public PageOrientation? Orientation { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginTop { get; set; }
        public int? MarginRight { get; set; }
        public int? MarginBottom { get; set; }
        public int? FooterMargin { get; set; }
        public int? HeaderMargin { get; set; }
        public PageSize Size { get; set; } = PageSize.A4;
        public Table Footer { get; set; }
        public readonly List<IPageElement> Elements = new List<IPageElement>();
    }
}