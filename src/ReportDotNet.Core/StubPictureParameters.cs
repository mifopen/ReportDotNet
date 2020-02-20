using System.Drawing;
using System.IO;

namespace ReportDotNet.Core
{
    public class StubPictureParameters
    {
        public byte[] Bytes
        {
            get
            {
                var bitmap = new Bitmap(MaxWidth, MaxHeight);
                for (var i = 0; i < bitmap.Width; i++)
                for (var j = 0; j < bitmap.Height; j++)
                    bitmap.SetPixel(i, j, Color.Black);
                return ImageToByte(bitmap);
            }
        }

        private static byte[] ImageToByte(Image img)
        {
            using var stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        public Color Color { get; set; } = Color.Black;
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public int? OffsetX { get; set; }
        public int? OffsetY { get; set; }
        public string Description { get; set; }
    }
}