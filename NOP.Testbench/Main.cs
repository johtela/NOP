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
				Tester.RunTestsTimed (
					new CollectionTests (),
					//new InterpreterTests (),
					new MapTests (),
					new SetTests (),
					new FingerTreeTests (),
					new TypeDefinitionTests (),
					new TypeCheckingTests (),
					//new ParserTests (),
					new SExprPathTests ())
			);
			Application.Run (VConsole);
			VConsole.Dispose ();
		}
	}
}
