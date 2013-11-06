namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using NOP;
	using NOP.IO;
	using NOP.Grammar;
	using NOP.Testing;

	public class IOTests
	{
		private static SExpr ReadFile (string fileName)
		{
			var stream = new FileStream (fileName, FileMode.Open);
			var reader = new StreamReader (stream);
			var store = new SExprTextStore ();
			return store.Read (reader);
		}

		[Test]
		public void TestSimpleLet ()
		{
			ParserTests.AssertParsesTo<Expression._Let> ("Int32", 
				ReadFile (@"SamplePrograms/SimpleLet.nop"));
		}

		[Test]
		public void TestSimpleLambda ()
		{
			ParserTests.AssertParsesTo<Expression._Let> ("String",
				ReadFile (@"SamplePrograms/SimpleLambda.nop"));
		}

		[Test]
		public void TestSimpleIf ()
		{
			ParserTests.AssertParsesTo<Expression._If> ("String",
				ReadFile (@"SamplePrograms/SimpleIf.nop"));
		}

		[Test]
		public void TestComplexIf ()
		{
			ParserTests.AssertParsesTo<Expression._Let> ("String",
				ReadFile (@"SamplePrograms/ComplexIf.nop"));
		}

		[Test]
		public void TestNestedLets ()
		{
			ParserTests.AssertParsesTo<Expression._Let> ("Boolean",
				ReadFile (@"SamplePrograms/NestedLets.nop"));
		}
		
		[Test]
		public void TestNestedLambdas ()
		{
			ParserTests.AssertParsesTo<Expression._Application> ("Boolean",
				ReadFile (@"SamplePrograms/NestedLambdas.nop"));
		}
	}
}
