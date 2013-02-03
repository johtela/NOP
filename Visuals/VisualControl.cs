namespace NOP.Visuals
{
	using System;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using NOP.Collections;

	public class VisualControl : Control
	{
		private Visual _visual;
		private VBox _size;
		private SExpr _code;
		private SExprPath _path;
		private NOPList<HitRect> _hitRects;

		public VisualControl ()
		{
			BackColor = Color.White;
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
				_path = new SExprPath ();
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

		protected override bool IsInputKey (System.Windows.Forms.Keys keyData)
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
			switch (e.KeyCode)
			{
				case Keys.Down:
					_path = _path.NextSibling (_code).Item2;
					break;
				case Keys.Up: 
					_path = _path.PrevSibling (_code).Item2;
					break;
				case Keys.Left:
					_path = _path.Previous (_code).Item2;
					break;
				case Keys.Right:
					_path = _path.Next (_code).Item2;
					break;
				default:
					base.OnKeyDown (e);
					return;
			}
			Invalidate ();
		}

		protected override void OnMouseDown (MouseEventArgs e)
		{
			base.OnMouseDown (e);
			var point = new PointF (e.X, e.Y);

			var hitRect = _hitRects.FindNext (hr => hr.Rect.Contains (point));
			if (hitRect.NotEmpty)
			{
				_path = new SExprPath (_code, hitRect.First.SExp);
				Invalidate ();
			}
		}

		protected override void OnPaint (PaintEventArgs pe)
		{
			base.OnPaint (pe);

			var focused = _path != null && _code != null ? _path.Target (_code) : null;
			if (_visual != null)
			{
				pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				var ctx = new GraphicsContext (pe.Graphics, focused);
				_visual.Render (ctx, _size);
				_hitRects = ctx.HitRects;
			}
		}
	}
}
