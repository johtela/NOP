namespace NOP.Testbench
{
	using NOP;
	using NOP.Grammar;
	using NOP.Testing;

	public class ParserTests : ExprUser
	{
		public static void AssertParsesTo<T> (string type, ExprBuilder eb) where T : Expression
		{
			AssertParsesTo<T> (type, eb.Build ());
		}

		public static void AssertParsesTo<T> (string type, SExpr sexp) where T : Expression
		{
			var expr = Expression.Parse (sexp);
			//System.Diagnostics.Debug.WriteLine (sexp);
			Check.IsOfType<T> (expr);
			Check.AreEqual (type, TC.InferType (expr.TypeCheck (), Bindings.Initial).ToString ());
			expr.ChangeVisualDepictions ();
			Runner.VConsole.ShowSExpr (sexp);
		}

		public static void AssertParsesToModule (ExprBuilder eb)
		{
			var sexp = eb.Build ();
			var module = Module.Parse (sexp);
			module.ChangeVisualDepictions ();
			Runner.VConsole.ShowSExpr (sexp);
		}

		[Test]
		public void TestAtom ()
		{
			AssertParsesTo<Expression._Literal> ("Int32", A (42));
		}
		
		[Test]
		public void TestLet ()
		{
			AssertParsesTo<Expression._Let> ("Int32", TestPrograms.SimpleLet ());
		}

		[Test]
		public void TestLambda ()
		{
			AssertParsesTo<Expression._Let> ("String", TestPrograms.SimpleLambda ());
		}

		[Test]
		public void TestIf ()
		{
			AssertParsesTo<Expression._If> ("String", TestPrograms.SimpleIf ());
		}

		[Test]
		public void TestComplexIf ()
		{
			AssertParsesTo<Expression._Let> ("String", TestPrograms.ComplexIf ());
		}

		[Test]
		public void TestNestedLets ()
		{
			AssertParsesTo<Expression._Let> ("Boolean", TestPrograms.NestedLets ());
		}

		[Test]
		public void TestNestedLambdas ()
		{
			AssertParsesTo<Expression._Application> ("Boolean", TestPrograms.NestedLambdas ());
		}

		[Test]
		public void TestSimpleModule ()
		{
			AssertParsesToModule (TestPrograms.SimpleModule ());
		}

		[Test]
		public void TestRecursiveFunction ()
		{
			AssertParsesTo<Expression._LetRec> ("StrictList<a> -> a -> Boolean", TestPrograms.RecursiveFunction ());
		}

		[Test]
		public void TestMutualRecursion ()
		{
			AssertParsesTo<Expression._LetRec> ("Int32 -> Boolean", TestPrograms.MutualRecursion ());
		}
	}
}