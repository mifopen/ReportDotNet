namespace ReportDotNet.Core
{
	public class Picture
	{
		public Picture(byte[] bytes,
					   int maxWidth,
					   int maxHeight,
					   int? offsetX = null,
					   int? offsetY = null)
		{
			Bytes = bytes;
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			OffsetX = offsetX;
			OffsetY = offsetY;
		}

		public byte[] Bytes { get; private set; }
		public int MaxWidth { get; private set; }
		public int MaxHeight { get; private set; }
		public int? OffsetX { get; private set; }
		public int? OffsetY { get; private set; }

		public bool IsEmpty()
		{
			return Bytes == null || Bytes.Length == 0;
		}
	}
}