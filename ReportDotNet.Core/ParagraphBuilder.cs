using System;
using System.Linq;

namespace ReportDotNet.Core
{
	public class ParagraphBuilder
	{
		private readonly ParagraphParameters parameters = new ParagraphParameters();

		public ParagraphBuilder FontSize(int fontSize) => Chain(p => p.FontSize = fontSize);
		public ParagraphBuilder Alignment(Alignment alignment) => Chain(p => p.Alignment = alignment);
		public ParagraphBuilder Bold(bool bold = true) => Chain(p => p.Bold = bold);
		public ParagraphBuilder SpaceBetweenLines(double spaceBetweenLines) => Chain(p => p.SpaceBetweenLines = spaceBetweenLines);
		public ParagraphBuilder AddText(string text) => Chain(p => p.Parts = p.Parts.Concat(new[] { new TextPart { Text = text } }));
		public ParagraphBuilder AddField(Field field) => Chain(p => p.Parts = p.Parts.Concat(new[] { new FieldPart { Field = field } }));
		public ParagraphBuilder AddPicture(Picture picture) => Chain(p => p.Parts = p.Parts.Concat(new[] { new PicturePart { Picture = picture } }));

		public Paragraph Build() => new Paragraph(parameters);

		public static implicit operator Paragraph(ParagraphBuilder builder) => builder.Build();

		private ParagraphBuilder Chain(Action<ParagraphParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}