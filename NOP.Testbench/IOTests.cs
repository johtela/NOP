namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.IO;
	using NOP;
	using NOP.IO;

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
			ParserTests.AssertParsesTo<LetExpression> ("System.Int32", 
				ReadFile (@"SamplePrograms\SimpleLet.nop"));
		}

		[Test]
		public void TestSimpleLambda ()
		{
			ParserTests.AssertParsesTo<LetExpression> ("System.String",
				ReadFile (@"SamplePrograms\SimpleLambda.nop"));
		}

		[Test]
		public void TestSimpleIf ()
		{
			ParserTests.AssertParsesTo<IfExpression> ("System.String",
				ReadFile (@"SamplePrograms\SimpleIf.nop"));
		}

		[Test]
		public void TestComplexIf ()
		{
			ParserTests.AssertParsesTo<LetExpression> ("System.String",
				ReadFile (@"SamplePrograms\ComplexIf.nop"));
		}

		[Test]
		public void TestNestedLambdas ()
		{
			ParserTests.AssertParsesTo<ApplicationExpression> ("System.Boolean",
				ReadFile (@"SamplePrograms\NestedLambdas.nop"));
		}

		[Test]
		public void TestNestedLets ()
		{
			ParserTests.AssertParsesTo<LetExpression> ("System.Boolean",
				ReadFile (@"SamplePrograms\NestedLets.nop"));
		}
	}
}
