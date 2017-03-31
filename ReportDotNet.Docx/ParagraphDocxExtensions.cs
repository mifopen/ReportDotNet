using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportDotNet.Core;
using FontFamily = System.Drawing.FontFamily;
using Paragraph = ReportDotNet.Core.Paragraph;

namespace ReportDotNet.Docx
{
	internal static class ParagraphDocxExtensions
	{
		public static OpenXmlElement Convert(this Paragraph paragraph, WordprocessingDocument document, int? defaultFontSize = null)
		{
			var docxParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
			var parameters = paragraph.Parameters;

			var paragraphProperties = new ParagraphProperties();

			if (parameters.Alignment.HasValue)
				paragraphProperties.Justification = new Justification { Val = GetJustificationValues(parameters.Alignment.Value) };

			if (parameters.SpaceBetweenLines.HasValue)
				paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines
														  {
															  LineRule = LineSpacingRuleValues.Auto,
															  Line = (240 * parameters.SpaceBetweenLines.Value).ToString()
														  };

			var runProperties = new RunProperties();
			if (parameters.Bold)
				runProperties.Bold = new Bold();

			var fontSize = parameters.FontSize ?? defaultFontSize;
			if (fontSize.HasValue)
			{
				var rPr = new ParagraphMarkRunProperties(new FontSize { Val = (fontSize.Value * 2).ToString() },
														 new FontSizeComplexScript { Val = (fontSize.Value * 2).ToString() });
				paragraphProperties.AppendChild(rPr);
				runProperties.FontSize = new FontSize { Val = (fontSize.Value * 2).ToString() };
				runProperties.FontSizeComplexScript = new FontSizeComplexScript { Val = (fontSize.Value * 2).ToString() };
			}

			var defaultFont = FontFamily.GenericSansSerif.Name;
			runProperties.RunFonts = new RunFonts
									 {
										 Ascii = defaultFont,
										 ComplexScript = defaultFont,
										 HighAnsi = defaultFont
									 };

			foreach (var run in parameters.Parts.Select(x => GetRun(document, x, runProperties)))
				docxParagraph.AppendChild(run);
			docxParagraph.ParagraphProperties = paragraphProperties;
			return docxParagraph;
		}

		private static Run GetRun(WordprocessingDocument document, Part part, RunProperties properties)
		{
			var run = new Run { RunProperties = properties.CloneNode<RunProperties>(true) };

			var textPart = part as TextPart;
			if (textPart != null)
			{
				var lines = textPart.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
				for (var i = 0; i < lines.Length; i++)
				{
					if (i > 0)
						run.AppendChild(new Break());
					if (!string.IsNullOrEmpty(lines[i]))
						run.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
				}

				return run;
			}

			var fieldPart = part as FieldPart;
			if (fieldPart != null)
			{
				run.AppendChild(new SimpleField { Instruction = GetInsructionForField(fieldPart.Field) });
				return run;
			}

			var picturePart = part as PicturePart;
			if (picturePart != null)
			{
				run.AppendChild(picturePart.Picture.Convert(document));
				return run;
			}

			throw new InvalidOperationException($"Can't process part of paragraph with type {part.GetType().FullName}");
		}

		private static JustificationValues GetJustificationValues(Alignment alignment)
		{
			switch (alignment)
			{
				case Alignment.Left:
					return JustificationValues.Left;
				case Alignment.Center:
					return JustificationValues.Center;
				case Alignment.Right:
					return JustificationValues.Right;
				case Alignment.Both:
					return JustificationValues.Both;
				default:
					throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
			}
		}

		private static string GetInsructionForField(Field field)
		{
			switch (field)
			{
				case Field.PageNumber:
					return " PAGE   \\* MERGEFORMAT ";
				case Field.PageCount:
					return " NUMPAGES   \\* MERGEFORMAT ";
				default:
					throw new ArgumentOutOfRangeException(nameof(field), field, null);
			}
		}
	}
}