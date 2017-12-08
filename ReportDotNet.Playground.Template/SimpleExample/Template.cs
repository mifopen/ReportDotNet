using System;
using System.Linq;
using ReportDotNet.Core;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Playground.Template.SimpleExample
{
	public class Data
	{
		public CashBookLine[] CashBookLines { get; set; }
	}

	public class CashBookLine
	{
		public string ContractorName { get; set; }
		public string DocumentNumber { get; set; }
		public string Expense { get; set; }
		public string Income { get; set; }
		public string DayTotalIncome { get; set; }
		public string DayTotalExpense { get; set; }
		public string BeginOfTheDaySum { get; set; }
		public string EndOfTheDaySum { get; set; }
		public string IncomeDocumentsCount { get; set; }
		public string ExpenseDocumentsCount { get; set; }
		public string SalaryPart { get; set; }
		public string PageNumber { get; set; }
		public DateTime Date { get; set; }
		public string CorrespondingAccount { get; set; }
	}

	public static class Template
	{
		public static void FillDocument(IDocument document,
										Action<object> log)
		{
			var data = new Data
					   {
						   CashBookLines = new[]
										   {
											   new CashBookLine
											   {
												   Date = new DateTime(2017, 1, 1),
												   ContractorName = "some name",
												   BeginOfTheDaySum = "123",
												   CorrespondingAccount = "asdf",
												   DayTotalExpense = "123",
												   DayTotalIncome = "123",
												   DocumentNumber = "2",
												   EndOfTheDaySum = "234",
												   Expense = "23",
												   ExpenseDocumentsCount = "2",
												   Income = "23",
												   IncomeDocumentsCount = "4",
												   PageNumber = "2",
												   SalaryPart = "32"
											   }
										   }
					   };
			var linesGroupedByPages = data.CashBookLines
										  .GroupBy(x => x.PageNumber)
										  .OrderBy(x => int.Parse(x.Key));
			foreach (var pageLines in linesGroupedByPages)
				document.AddPage(GetCashBookPage(pageLines.Key, pageLines.ToArray()));
		}

		private static Page GetCashBookPage(string pageNumber,
											CashBookLine[] lines)
		{
			var firstLine = lines.First();
			var widths = new int?[] { 75, null, 100, 100, 100 };
			return Page()
				.Orientation(PageOrientation.Portrait)
				.Margin(left: 100)
				.Add(Table(600)
						 .FontSize(10)
						 .Add(Row(Cell("Касса за", widths[0])
									  .Alignment(Alignment.Right),
								  Cell(firstLine.Date.ToString("dd.MM.yyyy"), 100)
									  .Borders(Borders.Bottom)
									  .Alignment(Alignment.Center),
								  Cell("г.", 30),
								  Cell("Лист")
									  .Alignment(Alignment.Right),
								  Cell(pageNumber, widths[4])
									  .Alignment(Alignment.Center)
									  .Borders(Borders.Bottom)),
							  Row()));
		}
	}
}