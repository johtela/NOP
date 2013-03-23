namespace NOP.Visuals
{
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using NOP.Collections;

	public class GraphicsContext
	{
		public readonly Graphics Graphics;
		public readonly SExpr FocusedExpr;
		public readonly bool EditMode;
		public readonly VisualStyle Style;
		public static NOPList<HitRect> HitRects;

		public GraphicsContext (Graphics gr, SExpr focused, VisualStyle style)
		{
			Graphics = gr;
			FocusedExpr = focused;
			Style = style;
		}

		public GraphicsContext (Graphics gr) : 
			this (gr, null, VisualStyle.Default) {}

		public GraphicsContext (GraphicsContext gc, VisualStyle style) : 
			this (gc.Graphics, gc.FocusedExpr, style) {}
	}
}

