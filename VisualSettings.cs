using System;
using System.Drawing;

namespace NOP
{
    public sealed class VisualSettings
    {
        public readonly Font Font;
        public readonly Pen Pen;
        public readonly Brush Brush;

        public VisualSettings(Font font, Pen pen, Brush brush)
        {
            Font = font;
            Pen = pen;
            Brush = brush;
        }
    }
}
