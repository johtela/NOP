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
			Check.AreEqual (type, expr.GetTypeExpr ().InferType (TypeEnv.Initial).ToString ());
			expr.ChangeVisualDepictions ();
			Runner.VConsole.ShowSExpr (sexp);
		}

		[Test]
		public void TestAtom ()
		{
			AssertParsesTo<Expression._Literal> ("System.Int32", A (42));
		}
		
		[Test]
		public void TestLet ()
		{
			AssertParsesTo<Expression._Let> ("System.Int32", TestPrograms.SimpleLet ());
		}

		[Test]
		public void TestLambda ()
		{
			AssertParsesTo<Expression._Let> ("System.String", TestPrograms.SimpleLambda ());
		}

		[Test]
		public void TestIf ()
		{
			AssertParsesTo<Expression._If> ("System.String", TestPrograms.SimpleIf ());
		}

		[Test]
		public void TestComplexIf ()
		{
			AssertParsesTo<Expression._Let> ("System.String", TestPrograms.ComplexIf ());
		}

		[Test]
		public void TestNestedLets ()
		{
			AssertParsesTo<Expression._Let> ("System.Boolean", TestPrograms.NestedLets ());
		}

		[Test]
		public void TestNestedLambdas ()
		{
			AssertParsesTo<Expression._Application> ("System.Boolean", TestPrograms.NestedLambdas ());
		}
	}
}