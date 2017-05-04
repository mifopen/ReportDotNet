using System;

namespace ReportDotNet.Core
{
    [Flags]
    public enum Borders
    {
        None = 0,
        Top = 1 << 0,
        Left = 1 << 1,
        Bottom = 1 << 2,
        Right = 1 << 3,
        All = Top | Left | Bottom | Right
    }
}