namespace NOP.Visuals
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;
	using NOP.Collections;

	public class HitRect
	{
		public readonly RectangleF Rect;
		public readonly SExpr SExp;

		public HitRect (RectangleF rect, SExpr sexp)
		{
			Rect = rect;
			SExp = sexp;
		}
	}
}