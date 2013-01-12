﻿namespace NOP.Testing
{
    using System.Drawing;
    using System.Windows.Forms;
    using System.Threading;
    using Collections;
    using Visuals;

    public class VisualConsole : Form
    {
        private NOPList<Visual> _visuals = NOPList<Visual>.Empty;
        private VisualControl _control;
      
        public VisualConsole ()
        {
            Parent = null;
            Text = "Visual Console";
            Size = new Size (700, 500);
            AutoScroll = true;
            _control = new VisualControl ();
            _control.Location = new Point (0, 0);
            _control.Parent = this;
        }

        public void ShowVisual(Visual visual)
        {
            lock (_visuals)
            {
                _visuals = visual | _visuals;
                _control.Visual = Visual.VStack (HAlign.Left,
                    _visuals.Collect (v => List.Create (v, Visual.HRuler ())));
                Invalidate ();
            }
        }
    }
}
