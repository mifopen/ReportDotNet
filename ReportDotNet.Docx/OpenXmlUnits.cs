using System;

namespace ReportDotNet.Docx
{
	internal static class OpenXmlUnits
	{
		public const int Dxa = 15;
		public const int EmuPerPixel = 9525;

		public static int FromMmTo20thOfPoint(double mm)
		{
			return (int) Math.Round(mm/10/2.54*72*20, MidpointRounding.AwayFromZero);
		}
	}
}