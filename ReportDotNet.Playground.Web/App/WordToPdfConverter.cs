using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace ReportDotNet.Web.App
{
	public class WordToPdfConverter
	{
		private readonly Lazy<Application> application = new Lazy<Application>(() => new Application());

		public byte[] Convert(byte[] source)
		{
			CleanOldWords();

			var inputFile = Path.GetTempFileName();
			var outputFile = Path.GetTempFileName();
			try
			{
				File.WriteAllBytes(inputFile, source);
				ConvertInternal(inputFile, outputFile);
				return File.ReadAllBytes(outputFile);
			}
			finally
			{
				File.Delete(inputFile);
				File.Delete(outputFile);
			}
		}

		private void CleanOldWords()
		{
			if (application.IsValueCreated)
				return;

			foreach (var p in Process.GetProcessesByName("winword"))
			{
				p.Kill();
				p.WaitForExit();
			}
		}

		private void ConvertInternal(string inputFile, string outputFile)
		{
			object oSource = inputFile;
			_Document document = application.Value.Documents.Open(ref oSource);

			if (document == null)
				throw new FileNotFoundException("incorrect input filename", inputFile);
			try
			{
				document.ActiveWindow.View.Type = WdViewType.wdNormalView;
				object saveFormat = WdSaveFormat.wdFormatPDF;
				object oDestination = outputFile;
				document.SaveAs(ref oDestination, ref saveFormat);
			}
			finally
			{
				object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
				document.Close(ref saveChanges);
			}
		}
	}
}