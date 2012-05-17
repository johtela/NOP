namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using NOP;
	
	class Runner
	{
		public static void Main (string[] args)
		{
			Tester.RunTests (
				new CollectionTests (),
				new InterpreterTests (),
				new MapTests (),
				new SetTests (),
				new TypeDefinitionTests ());
		}
	}
}
