using System;
using System.Drawing;

namespace ReportDotNet.Core
{
    public class StubPictureBuilder
    {
        private readonly StubPictureParameters parameters = new StubPictureParameters();

        public StubPictureBuilder Color(Color color) => Chain(p => p.Color = color);
        public StubPictureBuilder MaxWidth(int maxWidth) => Chain(p => p.MaxWidth = maxWidth);
        public StubPictureBuilder MaxHeight(int maxHeight) => Chain(p => p.MaxHeight = maxHeight);
        public StubPictureBuilder OffsetX(int? offsetX) => Chain(p => p.OffsetX = offsetX);
        public StubPictureBuilder OffsetY(int? offsetY) => Chain(p => p.OffsetY = offsetY);
        public StubPictureBuilder Description(string description) => Chain(p => p.Description = description);

        public StubPicture Build() => new StubPicture(parameters);

        public static implicit operator StubPicture(StubPictureBuilder builder) =>
            builder.Build();

        private StubPictureBuilder Chain(Action<StubPictureParameters> action)
        {
            action(parameters);
            return this;
        }
    }
}