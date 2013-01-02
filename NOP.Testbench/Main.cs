namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using NOP;
    using NOP.Testing;
	
	class Runner
	{
		public static void Main (string[] args)
		{
			Tester.RunTests (
				new CollectionTests (),
//				new InterpreterTests (),
				new MapTests (),
				new SetTests (),
				new TypeDefinitionTests (),
				new TypeCheckingTests (),
				new ParserTests ());

            using (var tf = new VisualTestWindow (null))
            {
                tf.ShowDialog ();
            }
        }
	}
}
