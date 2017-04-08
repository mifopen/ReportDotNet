using System;
using System.IO;
using ReportDotNet.Core;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Playground.Template.StampAndSignsExample
{
	public static class Template
	{
		public static void FillDocument(IDocument document, Action<object> log, string currentTemplateDirectory)
		{
			log(currentTemplateDirectory);
			FillDocument(document, new Data
								   {
									   AccountantName = "Accountant Name",
									   AccountantSign = File.ReadAllBytes(Path.Combine(currentTemplateDirectory, "AccountantSign.png")),
									   BossName = "Boss Name",
									   BossPosition = "Boss Position",
									   BossSign = File.ReadAllBytes(Path.Combine(currentTemplateDirectory, "BossSign.png")),
									   HasNoAccountant = false,
									   Stamp = File.ReadAllBytes(Path.Combine(currentTemplateDirectory, "Stamp.png"))
								   });
		}

		private static void FillDocument(IDocument document, Data data)
		{
			document.AddPage(Page()
								 .Orientation(PageOrientation.Portrait)
								 .Margin(10, 0, 0, 0)
								 .Size(new PageSize
									   {
										   Width = 143,
										   Height = 51
									   })
								 .Add(Table(545)
										  .FontSize(9)
										  .Add(Row()
												   .Height(40)
												   .Add(Cell(data.BossPosition, 280)
															.Borders(Borders.Bottom)
															.Alignment(Alignment.Center),
														Cell(80)
															.Borders(Borders.Bottom)
															.Add(Paragraph()
																	 .Add(new Picture(data.BossSign, 100, 45, offsetX: 10))),
														Cell(20)
															.Add(Paragraph()
																	 .Add(new Picture(data.Stamp, 170, 170,
																					  offsetX: -100,
																					  offsetY: 20))),
														Cell(data.BossName)
															.Borders(Borders.Bottom)
															.Alignment(Alignment.Center)
													   ),
											   Row(RemarkCell("(position)")
													   .Width(280),
												   RemarkCell("(signature)")
													   .Width(80),
												   Cell(20),
												   RemarkCell("(full name)")),
											   data.HasNoAccountant
												   ? Row()
												   : Row()
													   .Height(40)
													   .Add(Cell(280),
															Cell(80)
																.Add(Paragraph()
																		 .Add(new Picture(data.AccountantSign, 100, 45, offsetX: 10)))
																.Borders(Borders.Bottom),
															Cell(20),
															Cell(data.AccountantName)
																.Borders(Borders.Bottom)
																.Alignment(Alignment.Center)),
											   data.HasNoAccountant
												   ? Row()
												   : Row(Cell(280),
														 RemarkCell("(signature)")
															 .Width(80),
														 Cell(20),
														 RemarkCell("(full name)"))
											  )));
		}

		// Cell style reusing example
		private static CellBuilder RemarkCell(string text)
		{
			return Cell(text)
				.Alignment(Alignment.Center)
				.VerticalAlignment(VerticalAlignment.Top)
				.FontSize(7);
		}
	}
}