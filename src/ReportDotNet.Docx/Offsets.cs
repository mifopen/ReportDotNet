namespace ReportDotNet.Docx
{
    internal class Offsets
    {
        private Offsets(int? top,
                        int? right,
                        int? bottom,
                        int? left)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public static Offsets VerticalCenter(int verticalOffsetEachSide)
        {
            return new Offsets(NullIfZero(verticalOffsetEachSide),
                               null,
                               NullIfZero(verticalOffsetEachSide),
                               null);
        }

        public static Offsets HorizontalCenter(int horizontalOffsetEachSide)
        {
            return new Offsets(null,
                               NullIfZero(horizontalOffsetEachSide),
                               null,
                               NullIfZero(horizontalOffsetEachSide));
        }

        private static int? NullIfZero(int offset)
        {
            return offset == 0 ? (int?) null : offset;
        }

        public int? Top { get; private set; }
        public int? Right { get; private set; }
        public int? Bottom { get; private set; }
        public int? Left { get; private set; }
    }
}