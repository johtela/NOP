namespace NOP.UI
{
	using System;
	using Cairo;
	using Gtk;
	using NOP;
	
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
				var visual = NOP.Visual.Label ("Hello world");
				
				var vb = visual.CalculateSize (context);
				visual.Draw (context, vb);
			}
		}
	}
}