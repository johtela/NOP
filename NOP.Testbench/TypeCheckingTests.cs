namespace NOP.Testbench
{
	using NOP;
	using NOP.Testing;

	public class TypeCheckingTests : TypeExpr.Builder
	{
		private void CheckType (TypeExpr expr, string type)
		{
			Check.AreEqual (expr.InferType (TypeEnv.Initial).ToString (), type);
		}
		
		[Test]
		public void TestLteral ()
		{
			CheckType (Lit ("foo"), "String");
		}
		
		[Test]
		public void TestIdentityLambda ()
		{
			CheckType (Lam ("x", Var ("x")), "a -> a");
		}
		
		[Test]
		public void TestFirstAndSecondLambda ()
		{
			CheckType (Lam ("x", Lam ("y", Var ("x"))), "a -> b -> a");		
			CheckType (Lam ("x", Lam ("y", Var ("y"))), "a -> b -> b");		
		}
		
		[Test]
		public void TestFlipLambda ()
		{
			CheckType (Lam ("f", Lam ("a", Lam ("b", App (App (Var ("f"), Var ("b")), Var ("a"))))), 
			           "a -> b -> c -> b -> a -> c");		
		}
		
		[Test]
		public void SimpleLetTest ()
		{
			CheckType (Lam ("x", Let ("y", Lit (1), Var ("y"))), "a -> Int32");
		}
		
		[Test]
		public void ComplexLetTest ()
		{
			CheckType (Let ("f", Lam ("x", Var ("x")), App (Var ("f"), Lit (1))), "Int32");
		}
		
		[Test]
		public void SetTest ()
		{
			CheckType (Let ("a", Lit (1), Let ("b", Lit (2), App (App (Var ("set!"), Var ("a")), Lit (1)))), 
			           "Void");
		}
		
		[Test]
		public void SetTestFail ()
		{
			var expr = Let ("a", Lit (1), Let ("b", Lit (2), App (App (Var ("set!"), Var ("a")), Lit ("foo"))));
			Check.Throws<UnificationError> (() => expr.InferType (TypeEnv.Initial));
		}
		
		[Test]
		public void IfTest ()
		{
			CheckType (If (Lit (false), Lit ("foo"), Lit ("bar")), "String");
		}
	}
}