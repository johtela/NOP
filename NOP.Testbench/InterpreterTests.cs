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
		
		[Test]
		public void TestFunctionCall ()
		{
			AssertEvaluatesTo (3,
				Call (Definition.Get<Arithmetic, Function> ("Add(Int32, Int32)"), A (1), A (2)));
		}
		
		[Test]
		public void TestValue ()
		{
			AssertEvaluatesTo (Math.PI, A (Definition.Get<Arithmetic, Value> ("Pi"))); 
		}
		
		[Test]
		public void TestVariable ()
		{
			var baseVar = Definition.Get<Arithmetic, Variable> ("Base");
			
			AssertEvaluatesTo (16,
				Set (baseVar, A (16)),
				A (baseVar));
		}

		[Test]
		public void TestMethod ()
		{
			var method = Definition.Get<Number, Method> ("Add(NOP.Testbench.Number)");
			var num1 = new Number (3);
			var num2 = new Number (4);
			
			AssertEvaluatesTo (7,
				Call (num1, method, A (num2)));
		}
		
		[Test]
		public void TestProperty ()
		{
			var prop = Definition.Get<Number, Property> ("ValueSquared");
			var num1 = new Number (3);
			
			AssertEvaluatesTo(9, Prop (num1, prop));
		}
	}
}