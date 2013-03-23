namespace NOP.Visuals
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NOP.Collections;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Text;
	
	public class GraphicsContext
	{
		public readonly Graphics Graphics;
		public readonly SExpr FocusedExpr;
		public readonly bool EditMode;
		public Font DefaultFont;
		public Brush DefaultBrush;
		public Pen RectPen;
		public static NOPList<HitRect> HitRects;

		public GraphicsContext (Graphics gr, SExpr focused)
		{
			Graphics = gr;
			FocusedExpr = focused;
			DefaultFont = new Font ("DejaVu Sans Mono", 11);
			DefaultBrush = Brushes.Black;
			RectPen = new Pen (Color.Gray, 1) { DashStyle = DashStyle.Dot };
		}

		public GraphicsContext (Graphics gr) : this (gr, null) { }
	}
}

