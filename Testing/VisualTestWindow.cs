namespace NOP.Testing
{
    using System.Drawing;
    using System.Windows.Forms;
    using V = NOP.Visual;

    public class VisualTestWindow : Form
    {
        private Visual _visual;

        public VisualTestWindow (Visual visual)
        {
            Size = new Size (300, 200);
            _visual = visual ?? TestVisual();
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            var vSize = new VisualBox (ClientSize.Width, ClientSize.Height);
            _visual.Draw (e.Graphics, vSize);
        }
    
        private Visual TestVisual ()
        {
            return V.VerticalStack (HAlign.Left,
                V.HorizontalStack (VAlign.Bottom, V.Label ("Hello"), V.Label (" "), V.Label ("world!")),
                V.HorizontalStack (VAlign.Bottom, V.Label ("Pump "), V.Label ("up "), V.Label ("the volume!")));
        }
    }
}
