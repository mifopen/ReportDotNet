using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using Table = ReportDotNet.Core.Table;

namespace ReportDotNet.Docx.Converters
{
    internal static class TableConverter
    {
        public static OpenXmlElement Convert(Table table,
                                             WordprocessingDocument document)
        {
            var parameters = table.Parameters;
            var widths = Helpers.GetWidths(parameters.Width, parameters.Rows.Select(x => x.GetWidths()));
            var docxTable = CreateTable(parameters.Width, widths);
            foreach (var row in parameters.Rows)
                docxTable.AppendChild(RowConverter.Convert(row, document, table, widths));
            return docxTable;
        }

        private static DocumentFormat.OpenXml.Wordprocessing.Table CreateTable(float width,
                                                                               int[] columnWidths)
        {
            var table = new DocumentFormat.OpenXml.Wordprocessing.Table();
            var tableProperties = new TableProperties
                                  {
                                      TableWidth = new TableWidth
                                                   {
                                                       Width = (width * OpenXmlUnits.Dxa).ToString(),
                                                       Type = TableWidthUnitValues.Dxa
                                                   },
                                      TableLayout = new TableLayout
                                                    {
                                                        Type = TableLayoutValues.Fixed
                                                    }
                                  };
            table.AppendChild(tableProperties);
            table.AppendChild(new TableGrid(columnWidths.Select(x => new GridColumn { Width = (x * OpenXmlUnits.Dxa).ToString() })));
            return table;
        }
    }
}