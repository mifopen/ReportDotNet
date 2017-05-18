using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace ReportDotNet.Web.App
{
    public class WordToPdfConverter
    {
        private Application application;

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
            try
            {
                if (application != null)
                    return;

                foreach (var p in Process.GetProcessesByName("winword"))
                {
                    p.Kill();
                    p.WaitForExit();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void ConvertInternal(string inputFile,
                                     string outputFile)
        {
            object oSource = inputFile;
            var document = GetDocuments().Open(ref oSource);
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

        private Documents GetDocuments()
        {
            lock (this)
            {
                try
                {
                    if (application == null)
                        application = new Application();
                    return application.Documents;
                }
                catch (COMException e) when (e.Message.Contains("The RPC server is unavailable"))
                {
                    application = new Application();
                    return application.Documents;
                }
            }
        }
    }
}