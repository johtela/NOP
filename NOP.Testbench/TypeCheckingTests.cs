namespace NOP.Testbench
{
	using System;
	using NOP;
	using NOP.Collections;
	
	public class TypeCheckingTests : TypeExpr.Builder
	{
		[Test]
		public void TestLteral ()
		{
			var expr = Lit ("foo");
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString (), "System.String []");
		}
		
		[Test]
		public void TestIdentityLambda ()
		{
			var expr = Lam ("x", Var ("x"));
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString (), "a -> a");
		}
		
		[Test]
		public void TestFirstAndSecondLambda ()
		{
			var expr = Lam ("x", Lam ("y", Var ("x")));
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString (), "a -> b -> a");		

			var expr2 = Lam ("x", Lam ("y", Var ("y")));
			Check.AreEqual (expr2.GetExprType (TypeEnv.Initial).ToString (), "a -> b -> b");		
		}
		
		[Test]
		public void TestFlipLambda ()
		{
			var expr = Lam ("f", Lam ("a", Lam ("b", App (App (Var ("f"), Var ("b")), Var ("a")))));
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString (), "a -> b -> c -> b -> a -> c");		
		}
		
		[Test]
		public void SimpleLetTest ()
		{
			var expr = Lam ("x", Let ("y", Lit (1), Var ("y")));
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString(), "a -> System.Int32 []");
		}
		
		[Test]
		public void ComplexLetTest ()
		{
			var expr = Let ("f", Lam ("x", Var ("x")), App (Var ("f"), Lit (1)));
			Check.AreEqual (expr.GetExprType (TypeEnv.Initial).ToString (), "System.Int32 []");
		}
	}
}