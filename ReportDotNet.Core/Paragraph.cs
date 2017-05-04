namespace ReportDotNet.Core
{
    public class Paragraph: IPageElement
    {
        public ParagraphParameters Parameters { get; set; }

        public Paragraph(ParagraphParameters parameters)
        {
            Parameters = parameters;
        }
    }
}