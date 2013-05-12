namespace NOP.Testbench
{
	using NOP;

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
			AssertParsesTo<LiteralExpression> ("System.Int32", A (42));
		}
		
		[Test]
		public void TestLet ()
		{
			AssertParsesTo<LetExpression> ("System.Int32", TestPrograms.SimpleLet ());
		}

		[Test]
		public void TestLambda ()
		{
			AssertParsesTo<LetExpression> ("System.String", TestPrograms.SimpleLambda ());
		}

		[Test]
		public void TestIf ()
		{
			AssertParsesTo<IfExpression> ("System.String", TestPrograms.SimpleIf ());
		}

		[Test]
		public void TestComplexIf ()
		{
			AssertParsesTo<LetExpression> ("System.String", TestPrograms.ComplexIf ());
		}

		[Test]
		public void TestNestedLets ()
		{
			AssertParsesTo<LetExpression> ("System.Boolean", TestPrograms.NestedLets ());
		}

		[Test]
		public void TestNestedLambdas ()
		{
			AssertParsesTo<ApplicationExpression> ("System.Boolean", TestPrograms.NestedLambdas ());
		}
	}
}