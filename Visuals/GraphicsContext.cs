namespace NOP
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
		public Font DefaultFont;
		public Brush DefaultBrush;
		public Pen RectPen;


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

