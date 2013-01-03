namespace NOP.Testing
{
    using System.Drawing;
    using System.Windows.Forms;
    using System.Threading;
    using Collections;
    using V = NOP.Visual;

    public class VisualConsole : Form
    {
        private List<Visual> _visuals = List<Visual>.Empty;
        private PictureBox _canvas;
      
        public VisualConsole ()
        {
            Parent = null;
            Text = "Visual Console";
            BackColor = Color.White;
            Size = new Size (1000, 500);
            DoubleBuffered = true;
        }

        public void ShowVisual(Visual v)
        {
            lock (_visuals)
            {
                _visuals = v | _visuals;
            }
            Invalidate ();
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            foreach (var v in _visuals)
            {
                var st = e.Graphics.Save ();
                var size = v.CalculateSize (e.Graphics);
                v.Draw (e.Graphics, size);
                e.Graphics.Restore (st);
                e.Graphics.TranslateTransform (0, size.Height);
            }
        }

        private void InitializeComponent ()
        {
            this.SuspendLayout();
            // 
            // VisualConsole
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "VisualConsole";
            this.ResumeLayout(false);

        }    
    }
}
