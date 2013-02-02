namespace NOP.Testbench
{
	using System;
	using System.Linq;
	using NOP;
	using NOP.Collections;
	using NOP.Testing;

	public class ParserTests : ExprUser
	{
		private void AssertParsesTo<T> (string type, ExprBuilder eb) where T : Expression
		{
			AssertParsesTo<T> (type, eb.Build ());
		}

		private void AssertParsesTo<T> (string type, SExpr sexp) where T : Expression
		{
			var expr = Expression.Parse (sexp);
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
			AssertParsesTo<LetExpression> ("System.Int32", 
				Let ("x", A (33), S ("x")));
		}
		
		[Test]
		public void TestLambda ()
		{
			AssertParsesTo<LetExpression> ("System.String",
				Let ("getFoo", Lambda (P (), A ("foo")), Call ("getFoo")));
		}

		[Test]
		public void TestIf ()
		{
			AssertParsesTo<IfExpression> ("System.String",
				If (A (false), A ("foo"), A ("bar")));
		}

		[Test]
		public void TestComplexIf ()
		{
			AssertParsesTo<LetExpression> ("System.String",
				Let ("foo", Lambda (P ("i"), 
				If (Call ("eq?", S ("i"), A (3)), 
					A ("It's numberwang!"), 
					A ("It's a number"))
			),
				Call ("foo", A (3)))
			);
		}

		//[Test]
		//public void TestNestedLets ()
		//{
		//    AssertParsesTo<LetExpression> ("System.Boolean", 
		//       Let ("foo", A (42), 
		//         Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y"))),
		//         Call ("bar", S ("foo"), A (3)))
		//    )
		//    );
		//}

		//[Test]
		//public void TestNestedLambdas ()
		//{
		//    AssertParsesTo<ApplicationExpression> ("System.Boolean", SampleProgram ());
		//}

		[Test]
		public void TestSExprPath ()
		{
			var prog = SampleProgram ().Build ();
			var path = new SExprPath (List.Create (0, 2, 2, 1, 1));

			Check.AreEqual (path.Target (prog), S ("y").Build ());
			var prev = path.PrevSibling (prog);

			Check.AreEqual (prev.Item1, S ("x").Build ());
			Check.AreEqual (prev.Item2.PrevSibling (prog).Item1, L (S ("x"), S ("y")).Build ()); 

			path = new SExprPath (List.Create (0, 2));
			Check.AreEqual (path.NextSibling (prog).Item1, A (4).Build ());
		}

		private ExprBuilder SampleProgram ()
		{
			return Call (
				Lambda (P ("foo"),
					Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y"))),
					Call ("bar", S ("foo"), A (3)))
			), A (4));
		}
	}
}