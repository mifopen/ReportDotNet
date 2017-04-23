using System;

namespace ReportDotNet.Core
{
	public class PictureBuilder
	{
		private readonly PictureParameters parameters = new PictureParameters();

		public PictureBuilder Bytes(byte[] bytes) => Chain(p => p.Bytes = bytes);
		public PictureBuilder MaxWidth(int maxWidth) => Chain(p => p.MaxWidth = maxWidth);
		public PictureBuilder MaxHeight(int maxHeight) => Chain(p => p.MaxHeight = maxHeight);
		public PictureBuilder OffsetX(int? offsetX) => Chain(p => p.OffsetX = offsetX);
		public PictureBuilder OffsetY(int? offsetY) => Chain(p => p.OffsetY = offsetY);
		public PictureBuilder Description(string description) => Chain(p => p.Description = description);

		public Picture Build() => new Picture(parameters);

		public static implicit operator Picture(PictureBuilder builder) => builder.Build();

		private PictureBuilder Chain(Action<PictureParameters> action)
		{
			action(parameters);
			return this;
		}
	}
}