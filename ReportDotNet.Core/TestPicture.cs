using System.Drawing;

namespace ReportDotNet.Core
{
	public class TestPicture: Picture
	{
		public TestPicture(int maxWidth,
						   int maxHeight,
						   Color color = default(Color),
						   int? offsetX = null,
						   int? offsetY = null)
			: this(maxWidth,
				   maxHeight,
				   maxWidth,
				   maxHeight,
				   color,
				   offsetX,
				   offsetY)
		{
		}

		public TestPicture(int maxWidth,
						   int maxHeight,
						   int factWidth,
						   int factHeight,
						   Color color = default(Color),
						   int? offsetX = null,
						   int? offsetY = null)
			: base(GetTestBytes(factWidth, factHeight),
				   maxWidth,
				   maxHeight,
				   offsetX,
				   offsetY)
		{
			Color = color;
		}

		public Color Color { get; private set; }

		private static byte[] GetTestBytes(int width, int height)
		{
			var bitmap = new Bitmap(width, height);
			for (var i = 0; i < bitmap.Width; i++)
				for (var j = 0; j < bitmap.Height; j++)
					bitmap.SetPixel(i, j, Color.Black);
			return (byte[]) new ImageConverter().ConvertTo(bitmap, typeof (byte[]));
		}
	}
}