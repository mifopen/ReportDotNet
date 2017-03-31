using System;
using ReportDotNet.Core;
using static ReportDotNet.Core.Factories;

namespace ReportDotNet.Playground.Template
{
	public static class Template
	{
		public static void FillDocument(IDocument document, Action<object> log)
		{
			document.AddPage(Page()
								 .Add(Paragraph("START"),
									  Paragraph("\n\nsome textaha\n\nasdf\n\n"),
									  Paragraph("END")));

			var a = 234;
			var prolog = "";
			log(2);
			log(3);
			log(a);
			log(5);
			log(4);
		}
	}
}