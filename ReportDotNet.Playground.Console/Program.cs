using System.Diagnostics;
using System.IO;
using ReportDotNet.Core;
using ReportDotNet.Docx;

namespace ReportDotNet.Playground.Console
{
	internal class Program
	{
		private static void Main()
		{
			const string tempFileName = "ReportDotNet.Playground.Console.docx";
			if (File.Exists(tempFileName))
				File.Delete(tempFileName);
			var document = Create.Document.Docx();
			Template.Template.FillDocument(document, System.Console.Out.WriteLine);
			File.WriteAllBytes(tempFileName, document.Save());
			Process.Start(new ProcessStartInfo("winword.exe", tempFileName)
						  {
							  UseShellExecute = true
						  });
		}
	}
}