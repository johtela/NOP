﻿namespace NOP.Visuals
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Drawing.Text;
	using System.Windows.Forms;
	using NOP.Collections;

	public class VisualControl : Control
	{
		private Visual _visual;
		private VBox _size;
		private SExpr _code;
		private SExprPath _focusedPath;
		private StrictList<HitRect> _hitRects;
		private bool _editing;

		public VisualControl ()
		{
			BackColor = Color.Black;
			DoubleBuffered = true;
		}

		public Visual Visual
		{
			get { return _visual; }
			set
			{
				_visual = value;
				this.BeginInvoke (new Action (CalculateNewSize));
			}
		}

		public SExpr Code
		{
			get { return _code; }
			set
			{
				_code = value;
				Visual = Visual.Depiction (_code);
				_focusedPath = new SExprPath ();
			}
		}

		private void CalculateNewSize ()
		{
			if (_visual != null)
			{
				_size = _visual.GetSize (new GraphicsContext (Graphics.FromHwnd (Handle)));
				Width = Convert.ToInt32 (_size.Width);
				Height = Convert.ToInt32 (_size.Height);
			}
			Invalidate ();
		}

		protected override bool IsInputKey (Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Down:
				case Keys.Up:
				case Keys.Left:
				case Keys.Right:
					return true;
			}
			return base.IsInputKey (keyData);
		}

		protected override void OnKeyDown (KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				// Enter edit mode only if focused S-exp is not a list.
				if (!(GetFocusedExpr () is SExpr.List))
					_editing = !_editing;
			}
			else if (_editing)
			{

			}
			else Navigate (e.KeyCode);
		}

		private void Navigate (Keys key)
		{
			switch (key)
			{
				case Keys.Down:
					_focusedPath = _focusedPath.NextSibling (_code).Item2;
					break;
				case Keys.Up:
					_focusedPath = _focusedPath.PrevSibling (_code).Item2;
					break;
				case Keys.Left:
					_focusedPath = _focusedPath.Previous (_code).Item2;
					break;
				case Keys.Right:
					_focusedPath = _focusedPath.Next (_code).Item2;
					break;
			}
			Invalidate ();
		}

		private SExpr GetFocusedExpr ()
		{
			return _focusedPath != null && _code != null ? _focusedPath.Target (_code) : null;
		}

		protected override void OnMouseDown (MouseEventArgs e)
		{
			base.OnMouseDown (e);
			var point = new PointF (e.X, e.Y);

			var hitRect = _hitRects.FindNext (hr => hr.Rect.Contains (point));
			if (!hitRect.IsEmpty)
			{
				_focusedPath = new SExprPath (_code, hitRect.First.SExp);
				Invalidate ();
			}
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			base.OnPaint (pe);

			var focused = GetFocusedExpr ();
			if (_visual != null)
			{
				pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				var ctx = new GraphicsContext (pe.Graphics, focused, VisualStyle.Default);
				GraphicsContext.HitRects = StrictList<HitRect>.Empty;
				_visual.Render (ctx, _size);
				_hitRects = GraphicsContext.HitRects;
			}
		}
	}
}
