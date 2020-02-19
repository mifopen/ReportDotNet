using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using PageSize = DocumentFormat.OpenXml.Wordprocessing.PageSize;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Table = ReportDotNet.Core.Table;

namespace ReportDotNet.Docx.Converters
{
    internal static class PageConverter
    {
        public static OpenXmlElement[] Convert(Page page,
                                               IDocument document,
                                               WordprocessingDocument wpDocument,
                                               bool isLastPage)
        {
            var parts = page.Parameters.Elements.Select(p => PartToOpenXmlElement(p, wpDocument));

            if (isLastPage)
            {
                FillSectionProperties(wpDocument,
                                      wpDocument.MainDocumentPart.Document.Body.GetSectionProperties(),
                                      page,
                                      document.GetDefaultPageLayout(),
                                      document.GetDefaultFooter());
                return parts.ToArray();
            }

            return parts.Concat(new[]
                                {
                                    new Paragraph
                                    {
                                        ParagraphProperties = new ParagraphProperties
                                                              {
                                                                  SectionProperties = FillSectionProperties(wpDocument,
                                                                                                            new SectionProperties(),
                                                                                                            page,
                                                                                                            document.GetDefaultPageLayout(),
                                                                                                            document.GetDefaultFooter())
                                                              }
                                    }
                                })
                        .ToArray();
        }

        private static OpenXmlElement PartToOpenXmlElement(IPageElement part,
                                                           WordprocessingDocument document)
        {
            var paragraph = part as Core.Paragraph;
            if (paragraph != null)
                return ParagraphConverter.Convert(paragraph, document);

            var table = part as Table;
            if (table != null)
                return TableConverter.Convert(table, document);

            throw new InvalidOperationException($"can't convert part of page with type [{part.GetType()}] to OpenXmlElement");
        }

        private static SectionProperties FillSectionProperties(WordprocessingDocument wpDocument,
                                                               SectionProperties sectionProperties,
                                                               Page page,
                                                               PageLayout defaultPageLayout,
                                                               Table defaultFooter)
        {
            var pageOrientation = page.Parameters.Orientation ?? defaultPageLayout.Orientation ?? PageOrientation.Portrait;

            var width = (uint) OpenXmlUnits.FromMmTo20thOfPoint(page.Parameters.Size.Width);
            var height = (uint) OpenXmlUnits.FromMmTo20thOfPoint(page.Parameters.Size.Height);
            sectionProperties.AppendChild(new PageSize
                                          {
                                              Orient = ConvertOrientation(pageOrientation),
                                              Width = pageOrientation == PageOrientation.Portrait ? width : height,
                                              Height = pageOrientation == PageOrientation.Portrait ? height : width
                                          });
            sectionProperties.AppendChild(new PageMargin
                                          {
                                              Left = (uint?) GetMargin(page.Parameters.MarginLeft, defaultPageLayout.MarginLeft),
                                              Top = GetMargin(page.Parameters.MarginTop, defaultPageLayout.MarginTop),
                                              Right = (uint?) GetMargin(page.Parameters.MarginRight, defaultPageLayout.MarginRight),
                                              Bottom = GetMargin(page.Parameters.MarginBottom, defaultPageLayout.MarginBottom),
                                              Header = (uint?) GetMargin(page.Parameters.HeaderMargin, defaultPageLayout.HeaderMargin),
                                              Footer = (uint?) GetMargin(page.Parameters.FooterMargin, defaultPageLayout.FooterMargin)
                                          });
            if (page.Parameters.Footer != null || defaultFooter != null)
            {
                var footerPart = wpDocument.MainDocumentPart.AddNewPart<FooterPart>();
                var footerPartId = wpDocument.MainDocumentPart.GetIdOfPart(footerPart);
                sectionProperties.AppendChild(new FooterReference { Id = footerPartId, Type = HeaderFooterValues.Default });
                var footer = new Footer(TableConverter.Convert(page.Parameters.Footer ?? defaultFooter, wpDocument));
                footer.Save(footerPart);
            }
            return sectionProperties;
        }

        private static int? GetMargin(int? fromPage,
                                      int? fromDocument)
        {
            if (fromPage.HasValue || fromDocument.HasValue)
                return (fromPage ?? fromDocument) * OpenXmlUnits.Dxa;
            return null;
        }

        private static PageOrientationValues ConvertOrientation(PageOrientation orientation)
        {
            switch (orientation)
            {
                case PageOrientation.Portrait:
                    return PageOrientationValues.Portrait;
                case PageOrientation.Landscape:
                    return PageOrientationValues.Landscape;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}