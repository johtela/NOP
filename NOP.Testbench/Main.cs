namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using NOP;
	using NOP.Testing;
	using System.Windows.Forms;
	using System.Threading.Tasks;

	class Runner
	{
		public static VisualConsole VConsole = new VisualConsole ();

		[STAThread]
		public static void Main (string[] args)
		{
			Task.Factory.StartNew (() =>
				Tester.RunTests (
					//new CollectionTests (),
					//new LazyListTests (),
					//new MapTests (),
					//new SetTests (),
					//new FingerTreeTests (),
					//new TypeDefinitionTests (),
					//new TypeCheckingTests (),
					new ParserTests (),
					//new SExprPathTests (),
					//new ParserMonadTests (),
					new IOTests ())
			);
			Application.Run (VConsole);
			VConsole.Dispose ();
		}
	}
}
