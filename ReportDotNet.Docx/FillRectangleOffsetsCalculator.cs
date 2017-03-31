using System;
using System.Drawing;

namespace ReportDotNet.Docx
{
	internal static class FillRectangleOffsetsCalculator
	{
		private const int OpenxmlPercentMultiplier = 1000;

		public static Offsets Calculate(Size imageSize, Size placeholderSize)
		{
			var widthRatio = 1.0 * imageSize.Width / placeholderSize.Width;
			var heightRatio = 1.0 * imageSize.Height / placeholderSize.Height;

			return widthRatio > heightRatio
					   ? CenterVertical(imageSize, placeholderSize)
					   : CenterHorizontal(imageSize, placeholderSize);
		}

		private static Offsets CenterVertical(Size imageSize, Size placeholderSize)
		{
			var imageNewWidth = placeholderSize.Width;
			var imageNewHeight = imageSize.Height * imageNewWidth / imageSize.Width;

			var verticalOffset = (placeholderSize.Height - imageNewHeight) / 2.0;
			var verticalOffsetInWordPercents = PercentOf(verticalOffset, placeholderSize.Height) * OpenxmlPercentMultiplier;

			return Offsets.VerticalCenter(Convert.ToInt32(verticalOffsetInWordPercents));
		}

		private static Offsets CenterHorizontal(Size imageSize, Size placeholderSize)
		{
			var newImageHeight = placeholderSize.Height;
			var newImageWidth = imageSize.Width * newImageHeight / imageSize.Height;

			var horizontalOffsetInPixels = (placeholderSize.Width - newImageWidth) / 2.0;
			var horizontalOffsetInWordPercents = PercentOf(horizontalOffsetInPixels, placeholderSize.Width) * OpenxmlPercentMultiplier;

			return Offsets.HorizontalCenter(Convert.ToInt32(horizontalOffsetInWordPercents));
		}

		private static double PercentOf(double value, double of)
		{
			return value / of * 100;
		}
	}
}