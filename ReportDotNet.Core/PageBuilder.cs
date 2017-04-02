using System;

namespace ReportDotNet.Core
{
	public class PageBuilder
	{
		private readonly PageParameters parameters = new PageParameters();

		public PageBuilder Orientation(PageOrientation orientation) => Chain(p => p.Orientation = orientation);
		public PageBuilder HeaderMargin(int headerMargin) => Chain(p => p.HeaderMargin = headerMargin);
		public PageBuilder FooterMargin(int footerMargin) => Chain(p => p.FooterMargin = footerMargin);
		public PageBuilder Size(PageSize size) => Chain(p => p.Size = size);
		public PageBuilder Footer(Table footer) => Chain(p => p.Footer = footer);
		public PageBuilder Add(params Paragraph[] paragraphs) => Chain(p => p.Elements.AddRange(paragraphs));
		public PageBuilder Add(params Table[] tables) => Chain(p => p.Elements.AddRange(tables));

		public PageBuilder Margin(int? left = null,
								  int? top = null,
								  int? right = null,
								  int? bottom = null) => Chain(p =>
															   {
																   p.MarginLeft = left;
																   p.MarginTop = top;
																   p.MarginRight = right;
																   p.MarginBottom = bottom;
															   });

		public Page Build() => new Page(parameters);

		public static implicit operator Page(PageBuilder builder) => builder.Build();

		private PageBuilder Chain(Action<PageParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}