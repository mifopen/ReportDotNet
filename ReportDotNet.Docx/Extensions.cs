using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;

namespace ReportDotNet.Docx
{
	internal static class Extensions
	{
		public static string ToHex(this Color source)
		{
			return ByteToHex(source.R) + ByteToHex(source.G) + ByteToHex(source.B);
		}

		private static string ByteToHex(byte @byte)
		{
			var hex = @byte.ToString("X");
			return hex.Length < 2 ? "0" + hex : hex;
		}

		public static T CloneNode<T>(this T element, bool deep) where T:OpenXmlElement
		{
			return (T) element.CloneNode(deep);
		}

		public static SectionProperties GetSectionProperties(this Body body)
		{
			var sectionProperties = body.Elements<SectionProperties>().ToArray();
			return sectionProperties.Any()
					   ? sectionProperties.Single()
					   : body.AppendChild(new SectionProperties());
		}
	}
}