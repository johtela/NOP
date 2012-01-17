namespace NOP.Testbench
{
	using System;
	using System.Linq;
	using NOP;
	using NOP.Collections;

	public class InterpreterTests : ExprUser
	{
		private void AssertEvaluatesTo<T> (T expected, ExprBuilder expr)
		{
			Check.AreEqual ((object)expected, Interpreter.Evaluate (expr.Build ()));
		}
		
		private void AssertEvaluatesTo<T> (T expected, params ExprBuilder[] exprs)
		{
			Check.AreEqual ((object)expected, Interpreter.Evaluate (List.Create (exprs.Select (eb => eb.Build ()))));
			;
		}
		
		[Test]
		public void TestAtom ()
		{
			AssertEvaluatesTo (42, A (42));
		}
		
		[Test]
		public void TestDefine ()
		{
			AssertEvaluatesTo (33, 
				Define ("x", A (33)), 
				S ("x")
			);
		}
		
		[Test]
		public void TestLambda ()
		{
			AssertEvaluatesTo ("foo",
				Define ("getFoo", Lambda (P (), A ("foo"))),
				Call ("getFoo")
			);
		}

		[Test]
		public void TestBegin ()
		{
			AssertEvaluatesTo (33, 
				Begin (
					Define ("x", A (33)),
					S ("x")
				));
		}
		
		[Test]
		public void TestIf ()
		{
			AssertEvaluatesTo ("bar",
				If (A (false), 
					A ("foo"),
					A ("bar")
				));
		}
		
//		[Test]
		public void TestComparisons ()
		{
			AssertEvaluatesTo (true,
				Call ("<", A (1), A (2)));
			AssertEvaluatesTo (false, 
				Call (">", A (1), A (2)));
			AssertEvaluatesTo (true, 
				Call ("<=", A (1), A (1)));
			AssertEvaluatesTo (true, 
				Call (">=", A (1), A (1)));
		}
		
//		[Test]
		public void TestArithmetics ()
		{
			AssertEvaluatesTo (5,
				Call ("+", A (2), A (3)));
			AssertEvaluatesTo (0, 
				Call ("-", A (5), A (3), A (2)));
			AssertEvaluatesTo (3.3 * 3.0, 
				Call ("*", A (3.3), A (3.0)));
			AssertEvaluatesTo (5.0 / 2.0, 
				Call ("/", A (5.0), A (2.0)));
		}
	}
}