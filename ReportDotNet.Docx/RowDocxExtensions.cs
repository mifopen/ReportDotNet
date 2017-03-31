using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using Table = ReportDotNet.Core.Table;

namespace ReportDotNet.Docx
{
	internal static class RowDocxExtensions
	{
		public static TableRow Convert(this Row row, WordprocessingDocument document, Table table, int[] tableWidths)
		{
			var docxRow = new TableRow();
			var parameters = row.Parameters;
			if (parameters.Height.HasValue)
			{
				docxRow.TableRowProperties = new TableRowProperties();
				docxRow.TableRowProperties.AppendChild(new TableRowHeight
													   {
														   Val = (uint) parameters.Height.Value * OpenXmlUnits.Dxa,
														   HeightType = ConvertHeightType(parameters.HeightType)
													   });
			}

			var spans = Helpers.GetSpans(tableWidths, row.GetWidths());
			var cells = parameters.Cells.ToArray();
			for (var i = 0; i < cells.Length; i++)
				docxRow.AppendChild(cells[i].Convert(document, table, spans[i]));

			return docxRow;
		}

		private static EnumValue<HeightRuleValues> ConvertHeightType(RowHeightType heightType)
		{
			switch (heightType)
			{
				case RowHeightType.Exact:
					return HeightRuleValues.Exact;
				case RowHeightType.AtLeast:
					return HeightRuleValues.AtLeast;
				default:
					throw new ArgumentOutOfRangeException(nameof(heightType), heightType, null);
			}
		}
	}
}