using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using Color = System.Drawing.Color;
using Table = ReportDotNet.Core.Table;
using TextDirection = ReportDotNet.Core.TextDirection;

namespace ReportDotNet.Docx.Converters
{
    internal static class CellConverter
    {
        public static TableCell Convert(Cell cell,
                                        WordprocessingDocument document,
                                        Table table,
                                        int span)
        {
            var tableCell = new TableCell();
            var cellProperties = new TableCellProperties();
            var parameters = cell.Parameters;

            if (span > 1)
                cellProperties.GridSpan = new GridSpan { Val = span };

            cellProperties.TableCellVerticalAlignment = new TableCellVerticalAlignment
                                                        {
                                                            Val = GetVerticalAlignment(parameters.VerticalAlignment ?? VerticalAlignment.Bottom)
                                                        };

            cellProperties.TableCellMargin = GetMargin(parameters.MarginLeft ?? table.Parameters.CellMarginLeft, parameters.MarginRight ?? table.Parameters.CellMarginRight);

            if (parameters.MergeDown)
                cellProperties.VerticalMerge = new VerticalMerge { Val = MergedCellValues.Restart };
            else if (parameters.MergeUp)
                cellProperties.VerticalMerge = new VerticalMerge();

            var borders = parameters.Borders ?? table.Parameters.Borders ?? Borders.None;
            if (borders != Borders.None)
                cellProperties.TableCellBorders = new TableCellBorders
                                                  {
                                                      BottomBorder = borders.HasFlag(Borders.Bottom)
                                                                         ? GetBorder<BottomBorder>(parameters.BottomBorderStyle, parameters.BorderSize)
                                                                         : null,
                                                      LeftBorder = borders.HasFlag(Borders.Left)
                                                                       ? GetBorder<LeftBorder>(parameters.LeftBorderStyle, parameters.BorderSize)
                                                                       : null,
                                                      TopBorder = borders.HasFlag(Borders.Top)
                                                                      ? GetBorder<TopBorder>(parameters.TopBorderStyle, parameters.BorderSize)
                                                                      : null,
                                                      RightBorder = borders.HasFlag(Borders.Right)
                                                                        ? GetBorder<RightBorder>(parameters.RightBorderStyle, parameters.BorderSize)
                                                                        : null
                                                  };

            if (parameters.TextDirection.HasValue)
                cellProperties.TextDirection = new DocumentFormat.OpenXml.Wordprocessing.TextDirection { Val = ConvertTextDirection(parameters.TextDirection.Value) };

            if (parameters.BackgroundColor.HasValue)
                cellProperties.Shading = new Shading
                                         {
                                             Color = parameters.BackgroundColor.Value.ToHex(),
                                             Fill = "auto",
                                             Val = ShadingPatternValues.Solid
                                         };

            tableCell.AppendChild(ParagraphConverter.Convert(parameters.Paragraph, document, table.Parameters.FontSize));

            tableCell.TableCellProperties = cellProperties;
            return tableCell;
        }

        private static TBorder GetBorder<TBorder>(BorderStyle borderStyle,
                                                  double? borderSize)
            where TBorder: BorderType, new()
        {
            return new TBorder
                   {
                       Val = BorderStyleToBorderValues(borderStyle),
                       Color = Color.Black.ToHex(),
                       Size = (uint) ((borderSize ?? 0.5) * 4),
                       Space = 0
                   };
        }

        private static TextDirectionValues ConvertTextDirection(TextDirection textDirection)
        {
            switch (textDirection)
            {
                case TextDirection.LeftRight_TopBottom:
                    return TextDirectionValues.LefToRightTopToBottom;
                case TextDirection.RightLeft_TopBottom:
                    return TextDirectionValues.TopToBottomRightToLeft;
                case TextDirection.LeftRight_BottomTop:
                    return TextDirectionValues.BottomToTopLeftToRight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(textDirection), textDirection, null);
            }
        }

        private static BorderValues BorderStyleToBorderValues(BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.Single:
                    return BorderValues.Single;
                case BorderStyle.Dashed:
                    return BorderValues.Dashed;
                case BorderStyle.DotDash:
                    return BorderValues.DotDash;
                case BorderStyle.DotDotDash:
                    return BorderValues.DotDotDash;
                default:
                    throw new ArgumentOutOfRangeException(nameof(borderStyle), borderStyle, null);
            }
        }

        private static TableCellMargin GetMargin(int? left,
                                                 int? right)
        {
            if (left == null && right == null)
                return null;
            return new TableCellMargin
                   {
                       LeftMargin = left.HasValue
                                        ? new LeftMargin
                                          {
                                              Width = (left * OpenXmlUnits.Dxa).ToString(),
                                              Type = TableWidthUnitValues.Dxa
                                          }
                                        : null,
                       RightMargin = right.HasValue
                                         ? new RightMargin
                                           {
                                               Width = (right * OpenXmlUnits.Dxa).ToString(),
                                               Type = TableWidthUnitValues.Dxa
                                           }
                                         : null
                   };
        }

        private static TableVerticalAlignmentValues GetVerticalAlignment(VerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    return TableVerticalAlignmentValues.Top;
                case VerticalAlignment.Center:
                    return TableVerticalAlignmentValues.Center;
                case VerticalAlignment.Bottom:
                    return TableVerticalAlignmentValues.Bottom;
                default:
                    throw new ArgumentOutOfRangeException(nameof(verticalAlignment), verticalAlignment, null);
            }
        }
    }
}