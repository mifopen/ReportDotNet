namespace ReportDotNet.Core
{
	public class Picture
	{
		public PictureParameters Parameters { get; }

		public Picture(PictureParameters parameters)
		{
			Parameters = parameters;
		}

		public bool IsEmpty() => Parameters.Bytes == null || Parameters.Bytes.Length == 0;
	}
}