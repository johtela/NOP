using System;
using System.Drawing;

namespace NOP
{
    public static class VisualHelpers
    {
        public static Size Intersect(this Size size, Size with)
        {
            return new Size(Math.Min(size.Width, with.Width), Math.Min(size.Height, with.Height));
        }
    }
}
