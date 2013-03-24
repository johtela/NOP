﻿namespace NOP.Visuals
{
	using System.Drawing;
	using System.Drawing.Drawing2D;

	public class VisualStyle
	{
		private readonly VisualStyle _parent;
		private readonly Font _font;
		private readonly Brush _textBrush;
		private readonly Pen _pen;

		public static VisualStyle Default = new VisualStyle (
			font: new Font ("DejaVu Sans Mono", 11),
			brush: Brushes.Black,
			pen: new Pen (Color.Gray, 1) { DashStyle = DashStyle.Dash });

		public VisualStyle (VisualStyle parent = null, Font font = null, 
			Brush brush = null, Pen pen = null)
		{
			_parent = parent ?? Default;
			_font = font;
			_textBrush = brush;
			_pen = pen;
		}

		public VisualStyle Parent
		{
			get { return _parent; }
		}

		public Font Font
		{
			get
			{
				var vs = this;
				while (vs._font == null)
					vs = vs._parent;
				return vs._font;
			}
		}

		public Brush TextBrush
		{
			get
			{
				var vs = this;
				while (vs._textBrush == null)
					vs = vs._parent;
				return vs._textBrush;
			}
		}

		public Pen Pen
		{
			get
			{
				var vs = this;
				while (vs._pen == null)
					vs = vs._parent;
				return vs._pen;
			}
		}
	}
}