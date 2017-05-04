using System.Drawing;

namespace ReportDotNet.Core
{
    public class CellParameters
    {
        public Paragraph Paragraph { get; set; }
        public int? Width { get; set; }
        public VerticalAlignment? VerticalAlignment { get; set; }
        public Borders? Borders { get; set; }
        public int? MarginLeft { get; set; }
        public int? MarginRight { get; set; }
        public bool MergeDown { get; set; }
        public bool MergeUp { get; set; }
        public BorderStyle LeftBorderStyle { get; set; }
        public BorderStyle TopBorderStyle { get; set; }
        public BorderStyle RightBorderStyle { get; set; }
        public BorderStyle BottomBorderStyle { get; set; }
        public TextDirection? TextDirection { get; set; }
        public Color? BackgroundColor { get; set; }
        public double? BorderSize { get; set; }
    }
}