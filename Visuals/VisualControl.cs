namespace NOP.Visuals
{
	using System;
	using System.Windows.Forms;
	using System.Drawing;
	using System.Drawing.Drawing2D;

	public class VisualControl : Control
	{
		private Visual _visual;
		private VBox _size;
		private SExpr _code;
		private SExprPath _path;

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

		protected override void OnPaint (PaintEventArgs pe)
		{
			base.OnPaint (pe);

			var focused = _path != null && _code != null ? _path.Target (_code) : null;
			if (_visual != null)
			{
				pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				_visual.Render (new GraphicsContext (pe.Graphics, focused), _size);
			}
		}
	}
}
