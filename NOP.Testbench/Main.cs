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
        public static VisualConsole VConsole;

        [STAThread]
        public static void Main (string[] args)
        {
            Task.Factory.StartNew (() =>
                Tester.RunTests (
                    new CollectionTests (),
                    //new InterpreterTests (),
                    new MapTests (),
                    new SetTests (),
                    new TypeDefinitionTests (),
                    new TypeCheckingTests (),
                    new ParserTests ()));

            VConsole = new VisualConsole ();
            Application.Run (VConsole);
            VConsole.Dispose ();
        }
	}
}
