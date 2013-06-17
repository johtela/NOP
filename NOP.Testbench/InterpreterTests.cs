namespace NOP.Testbench
{
	using System;
	using System.Linq;
	using NOP;
	using NOP.Framework;
	using NOP.Collections;
	using NOP.Testing;

	public class InterpreterTests : ExprUser
	{
		private void AssertEvaluatesTo<T> (T expected, ExprBuilder expr)
		{
			Check.AreEqual ((object)expected, Interpreter.Evaluate (expr.Build ()));
		}
		
		private void AssertEvaluatesTo<T> (T expected, params ExprBuilder[] exprs)
		{
			Check.AreEqual ((object)expected, Interpreter.Evaluate (List.FromEnumerable (exprs.Select (eb => eb.Build ()))));
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
			)
			);
		}
		
		[Test]
		public void TestIf ()
		{
			AssertEvaluatesTo ("bar",
				If (A (false), 
					A ("foo"),
					A ("bar"))
			);
		}
	}
}