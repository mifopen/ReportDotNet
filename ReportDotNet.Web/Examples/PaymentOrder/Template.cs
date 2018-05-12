using System;
using System.Linq;
using ReportDotNet.Core;

namespace ReportDotNet.Web.Examples.PaymentOrder
{
    public static class Template
    {
        public static void FillDocument(IDocument document,
                                        Action<object> log)
        {
            FillDocument(document, new[]
                                   {
                                       new Data
                                       {
                                           Sum = 123
                                       },
                                       new Data
                                       {
                                           Sum = 456
                                       }
                                   });
        }

        private static void FillDocument(IDocument document,
                                         Data[] cashOrders)
        {
            foreach (var cashOrderInfo in cashOrders)
            {
                var page = GetPage(cashOrderInfo);
                document.AddPage(page.Margin(top: 50, left: 60));
            }
        }

        private static PageBuilder GetPage(Data data)
        {
            var widths = new[] { 30, 20, 60, 60, 50, 60, 60, 30 };
            Func<string, bool, bool, Cell[]> cutFunc = (s,
                                                        down,
                                                        up) =>
                                                           new[]
                                                               {
                                                                   Factories.Cell(5)
                                                                            .Margin(0, 0)
                                                                            .Borders(Borders.Right),
                                                                   Factories.Cell(s, 20)
                                                                            .FontSize(9)
                                                                            .TextDirection(TextDirection.RightLeft_TopBottom)
                                                                            .MergeDown(down)
                                                                            .MergeUp(up)
                                                                            .Borders(Borders.Left | Borders.Right)
                                                                            .Borders(right: BorderStyle.DotDotDash),
                                                                   Factories.Cell(5)
                                                                            .Margin(0, 0)
                                                                            .Borders(Borders.Left | Borders.Right)
                                                                            .Borders(left: BorderStyle.DotDotDash),
                                                                   Factories.Cell(5)
                                                                            .Margin(0, 0)
                                                                            .Borders(Borders.Left)
                                                               }.Select(x => x.Build())
                                                                .ToArray();
            var cut = cutFunc(null, false, false);

            return Factories.Page()
                            .Add(Factories.Table(630)
                                          .FontSize(7)
                                          .Add(Factories.Row(Factories.Cell(140),
                                                             Factories.Cell("Унифицированная форма N КО-1\nУтверждена постановлением Госкомстата\nРоссии от 18.08.98 № 88", 230))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(data.LegalName)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell(370))
                                                        .Add(cut)
                                                        .Add(remarkCell(null, "(организация)")),
                                               Factories.Row(Factories.Cell(270),
                                                             Factories.Cell("Код", 100)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All))
                                                        .Add(cut),
                                               Factories.Row(Factories.Cell(string.IsNullOrEmpty(data.LegalName33Second) ? null : data.LegalName33First, 170)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("Форма по ОКУД", 100)
                                                                      .Alignment(Alignment.Right),
                                                             Factories.Cell("0310001", 100)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("КВИТАНЦИЯ")
                                                                      .Bold()
                                                                      .FontSize(10)
                                                                      .Alignment(Alignment.Center)),
                                               Factories.Row(Factories.Cell(string.IsNullOrEmpty(data.LegalName33Second) ? data.LegalName33First : data.LegalName33Second, 170)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell("по ОКПО", 100)
                                                                      .Alignment(Alignment.Right),
                                                             Factories.Cell(data.Okpo, 100)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell()),
                                               Factories.Row(remarkCell(170, "(организация)"),
                                                             Factories.Cell(100),
                                                             Factories.Cell(100)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell()),
                                               Factories.Row(Factories.Cell(170)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(100),
                                                             Factories.Cell(100)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("к приходному кассовому ордеру №", 190),
                                                             Factories.Cell(data.DocumentNumber)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(remarkCell(170, "(структурное подразделение)"),
                                                             Factories.Cell(200))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("от", 20)
                                                                      .Margin(right: 0),
                                                             Factories.Cell("\"", 10)
                                                                      .Alignment(Alignment.Right)
                                                                      .Margin(right: 0),
                                                             Factories.Cell(data.Day, 30)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("\"", 10)
                                                                      .Margin(left: 0),
                                                             Factories.Cell(data.Month, 70)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell(10),
                                                             Factories.Cell(data.Year, 40)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("г.")),
                                               Factories.Row(Factories.Cell(220),
                                                             Factories.Cell("Номер документа", 75)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0)
                                                                      .Borders(Borders.All),
                                                             Factories.Cell("Дата составления", 75)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("Принято от", 75),
                                                             Factories.Cell(data.Contractor26First)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell("ПРИХОДНЫЙ КАССОВЫЙ ОРДЕР", 220)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0)
                                                                      .Bold()
                                                                      .FontSize(9),
                                                             Factories.Cell(data.DocumentNumber, 75)
                                                                      .Margin(0, 0)
                                                                      .Bold()
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(data.DateStr, 75)
                                                                      .Margin(0, 0)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(data.Contractor26Second)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell(370))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(data.Contractor26Third)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(headerCell(widths[0], "Дебет", true, false),
                                                             headerCell(widths[1] + widths[2] + widths[3] + widths[4], "Кредит", false, false),
                                                             headerCell(widths[5], "Сумма, руб. коп.", true, false),
                                                             headerCell(widths[6], "Код целевого назначение", true, false),
                                                             headerCell(widths[7], null, true, false))
                                                        .Add(cutFunc("Линия отреза", true, false))
                                                        .Add(Factories.Cell("Основание", 60)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(data.Cause26First)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(headerCell(widths[0], null, false, true),
                                                             headerCell(widths[1], null, true, false),
                                                             headerCell(widths[2], "код струк-турного под-разделения", true, false),
                                                             headerCell(widths[3], "корреспон-дирующий счет, субсчет", true, false),
                                                             headerCell(widths[4], "код аналити-ческого учета", true, false),
                                                             headerCell(widths[5], null, false, true),
                                                             headerCell(widths[6], null, false, true),
                                                             headerCell(widths[7], null, false, true))
                                                        .Add(cutFunc(null, false, true))
                                                        .Add(Factories.Cell(data.Cause26Second)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(headerCell(widths[0], null, false, true),
                                                             headerCell(widths[1], null, false, true),
                                                             headerCell(widths[3], null, false, true),
                                                             headerCell(widths[2], null, false, true),
                                                             headerCell(widths[4], null, false, true),
                                                             headerCell(widths[5], null, false, true),
                                                             headerCell(widths[6], null, false, true),
                                                             headerCell(widths[7], null, false, true))
                                                        .Add(cutFunc(null, false, true))
                                                        .Add(Factories.Cell(data.Cause26Third)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(headerCell(widths[0], null, false, true),
                                                             headerCell(widths[1], null, false, true),
                                                             headerCell(widths[2], null, false, true),
                                                             headerCell(widths[3], null, false, true),
                                                             headerCell(widths[4], null, false, true),
                                                             headerCell(widths[5], null, false, true),
                                                             headerCell(widths[6], null, false, true),
                                                             headerCell(widths[7], null, false, true))
                                                        .Add(cutFunc(null, false, true))
                                                        .Add(Factories.Cell().Borders(Borders.Bottom)),
                                               Factories.Row(30)
                                                        .Add(Factories.Cell("50", widths[0])
                                                                      .Alignment(Alignment.Center)
                                                                      .VerticalAlignment(VerticalAlignment.Center)
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(widths[1])
                                                                      .Borders(Borders.All),
                                                             Factories.Cell("-", widths[2])
                                                                      .VerticalAlignment(VerticalAlignment.Center)
                                                                      .Borders(Borders.All)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell(data.CorrespondingAccountingRecord, widths[3])
                                                                      .VerticalAlignment(VerticalAlignment.Center)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(widths[4])
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(data.SumStr, widths[5])
                                                                      .VerticalAlignment(VerticalAlignment.Center)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(widths[6])
                                                                      .Borders(Borders.All),
                                                             Factories.Cell(widths[7])
                                                                      .Borders(Borders.All))
                                                        .Add(cutFunc(null, false, true))
                                                        .Add(Factories.Cell("Сумма", 50),
                                                             Factories.Cell(data.SumRubles, 80)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("руб.", 30)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(data.SumCopecks, 30)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("коп.")),
                                               Factories.Row(Factories.Cell(370))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(50),
                                                             remarkCell(70, "(цифрами)"),
                                                             Factories.Cell()),
                                               Factories.Row(Factories.Cell(70),
                                                             Factories.Cell(string.IsNullOrEmpty(data.Contractor45Second) ? null : data.Contractor45First, 300))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(data.SumInWords36First)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell("Принято от", 70)
                                                                      .Margin(right: 0),
                                                             Factories.Cell(string.IsNullOrEmpty(data.Contractor45Second) ? data.Contractor45First : data.Contractor45Second, 300)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(remarkCell(null, "(прописью)")),
                                               Factories.Row(Factories.Cell(370))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(data.SumInWords36Second)
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell("Основание", 80),
                                                             Factories.Cell(data.Cause45First, 290)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(130)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell("руб.", 30)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(data.SumCopecks, 30)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("коп.")),
                                               Factories.Row(Factories.Cell(data.Cause45Second, 370)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("В том числе", 80),
                                                             Factories.Cell("без налога (НДС)")
                                                                      .Borders(Borders.Bottom)),
                                               Factories.Row(Factories.Cell("Сумма", 60),
                                                             Factories.Cell(data.SumInWords36First, 310)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(Factories.Cell()),
                                               Factories.Row(Factories.Cell(60),
                                                             remarkCell(310, "(прописью)"))
                                                        .Add(cut)
                                                        .Add(Factories.Cell()),
                                               Factories.Row(Factories.Cell(data.SumInWords36Second, 280)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell("руб.", 30)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(data.SumCopecks, 30)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell("коп.", 30)
                                                                      .Alignment(Alignment.Center)
                                                                      .Margin(0, 0))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("\"", 10)
                                                                      .Margin(right: 0)
                                                                      .Alignment(Alignment.Right),
                                                             Factories.Cell(data.Day, 30)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell("\"", 10)
                                                                      .Margin(left: 0)
                                                                      .Alignment(Alignment.Left),
                                                             Factories.Cell(data.Month, 80)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(5),
                                                             Factories.Cell(data.Year, 50)
                                                                      .Alignment(Alignment.Center)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell("г.")),
                                               Factories.Row(Factories.Cell("В том числе", 80),
                                                             Factories.Cell("без налога (НДС)", 290)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(Factories.Cell()),
                                               Factories.Row(30)
                                                        .Add(Factories.Cell("Приложение", 90),
                                                             Factories.Cell(data.Appendix, 280)
                                                                      .Borders(Borders.Bottom))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(40),
                                                             Factories.Cell("М.П.(штампа)")),
                                               Factories.Row(30)
                                                        .Add(Factories.Cell("Главный бухгалтер", 120),
                                                             Factories.Cell(80)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(10),
                                                             Factories.Cell(data.AccountantFio, 140)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell(20))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("Главный бухгалтер", 94)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(37)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(3)
                                                                      .Margin(0, 0),
                                                             Factories.Cell(data.AccountantFio)
                                                                      .Borders(Borders.Bottom)
                                                                      .Margin(0, 0)
                                                                      .Alignment(Alignment.Center)
                                                                      .FontSize(7)),
                                               Factories.Row(Factories.Cell(120),
                                                             remarkCell(80, "(подпись)"),
                                                             Factories.Cell(10),
                                                             remarkCell(140, "(расшифровка подписи)"),
                                                             Factories.Cell(20))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(94),
                                                             remarkCell(37, "(подпись)"),
                                                             Factories.Cell(3)
                                                                      .Margin(0, 0),
                                                             remarkCell(null, "(расшифровка подписи)")),
                                               Factories.Row(Factories.Cell("Получил кассир", 120),
                                                             Factories.Cell(80)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(10),
                                                             Factories.Cell(140)
                                                                      .Borders(Borders.Bottom)
                                                                      .Alignment(Alignment.Center),
                                                             Factories.Cell(20))
                                                        .Add(cut)
                                                        .Add(Factories.Cell("Кассир", 50)
                                                                      .Margin(left: 0),
                                                             Factories.Cell(40)
                                                                      .Borders(Borders.Bottom),
                                                             Factories.Cell(5)
                                                                      .Margin(0, 0),
                                                             Factories.Cell()
                                                                      .Borders(Borders.Bottom)
                                                                      .Margin(0, 0)
                                                                      .Alignment(Alignment.Center)
                                                                      .FontSize(7)),
                                               Factories.Row(Factories.Cell(120),
                                                             remarkCell(80, "(подпись)"),
                                                             Factories.Cell(10),
                                                             remarkCell(140, "(расшифровка подписи)"),
                                                             Factories.Cell(20))
                                                        .Add(cut)
                                                        .Add(Factories.Cell(50),
                                                             remarkCell(40, "(подпись)"),
                                                             Factories.Cell(5)
                                                                      .Margin(0, 0),
                                                             remarkCell(null, "(расшифровка подписи)"))));
        }

        private static readonly Func<int?, string, Cell> remarkCell = (w,
                                                                       s) =>
                                                                          Factories.Cell(s, w)
                                                                                   .VerticalAlignment(VerticalAlignment.Top)
                                                                                   .Alignment(Alignment.Center)
                                                                                   .Margin(0, 0)
                                                                                   .FontSize(6);

        private static readonly Func<int?, string, bool, bool, Cell> headerCell = (w,
                                                                                   s,
                                                                                   down,
                                                                                   up) =>
                                                                                      Factories.Cell(s, w)
                                                                                               .FontSize(7)
                                                                                               .Margin(0, 0)
                                                                                               .Alignment(Alignment.Center)
                                                                                               .VerticalAlignment(VerticalAlignment.Center)
                                                                                               .Borders(Borders.All)
                                                                                               .MergeDown()
                                                                                               .MergeUp();
    }
}