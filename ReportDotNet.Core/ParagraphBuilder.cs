using System;
using System.Drawing;

namespace ReportDotNet.Core
{
	public class ParagraphBuilder
	{
		private readonly ParagraphParameters parameters = new ParagraphParameters();

		public ParagraphBuilder FontSize(int fontSize) => Chain(p => p.FontSize = fontSize);
		public ParagraphBuilder Alignment(Alignment alignment) => Chain(p => p.Alignment = alignment);
		public ParagraphBuilder Bold(bool bold = true) => Chain(p => p.Bold = bold);
		public ParagraphBuilder SpaceBetweenLines(double spaceBetweenLines) => Chain(p => p.SpaceBetweenLines = spaceBetweenLines);
		public ParagraphBuilder Add(string text) => Chain(p => p.Parts.Add(new TextPart { Text = text }));
		public ParagraphBuilder Add(Field field) => Chain(p => p.Parts.Add(new FieldPart { Field = field }));
		public ParagraphBuilder Add(Picture picture) => Chain(p => p.Parts.Add(new PicturePart { Picture = picture }));
		public ParagraphBuilder Add(StubPicture stubPicture) => Chain(p => p.Parts.Add(new StubPicturePart { StubPicture = stubPicture }));
		public ParagraphBuilder BackgroundColor(Color color) => Chain(p => p.BackgroundColor = color);

		public Paragraph Build() => new Paragraph(parameters);

		public static implicit operator Paragraph(ParagraphBuilder builder) => builder.Build();

		private ParagraphBuilder Chain(Action<ParagraphParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}