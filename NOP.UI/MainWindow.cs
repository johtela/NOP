namespace NOP.UI
{
	using System;
	using Cairo;
	using Gtk;
	using V=NOP.Visual;
	
	public partial class MainWindow: Gtk.Window
	{	
		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
			Build ();
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	
		protected void OnDrawingareaExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			using (Context context = Gdk.CairoHelper.Create (drawingarea.GdkWindow))
			{
				var size = new VisualBox (args.Event.Area.Width, args.Event.Area.Height);
				
				context.SelectFontFace ("Consolas", FontSlant.Normal, FontWeight.Normal);
				context.SetFontSize (12);
				var visual = TestVisual ();
				
				visual.Draw (context, size);
				
#if __MonoCS__
				context.Target.Dispose ();
#endif
			}
		}
		
		private Visual TestVisual ()
		{
			return V.VerticalStack (HAlign.Left,
				V.HorizontalStack (VAlign.Bottom, V.Label ("Hello"), V.Label (" "), V.Label ("world!")),
				V.HorizontalStack (VAlign.Bottom, V.Label ("Pump "), V.Label ("up "), V.Label ("the volume!")));
		}
	}
}