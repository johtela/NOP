namespace NOP.Testbench
{
	using System;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using NOP.Testbench.Collections;
	using NOP.Testing;

	class Runner
	{
		public static VisualConsole VConsole = new VisualConsole ();

		[STAThread]
		public static void Main (string[] args)
		{
			Task.Factory.StartNew (() =>
				Tester.RunTestsTimed (
					new IStreamTests (),
					new ISequenceTests (),
					new StrictListTests (),
					new LazyListTests (),
					new MapTests (),
					new SetTests (),
					new FingerTreeTests (),
					new TypeCheckingTests (),
					new ParserTests (),
					new SExprPathTests (),
					new ParserMonadTests ())
					//new IOTests ())
			);
			Application.Run (VConsole);
			VConsole.Dispose ();
		}
	}
}
