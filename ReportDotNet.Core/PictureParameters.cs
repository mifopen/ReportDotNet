namespace ReportDotNet.Core
{
	public class PictureParameters
	{
		public byte[] Bytes { get; set; }
		public int MaxWidth { get; set; }
		public int MaxHeight { get; set; }
		public int? OffsetX { get; set; }
		public int? OffsetY { get; set; }
		public string Description { get; set; }
	}
}