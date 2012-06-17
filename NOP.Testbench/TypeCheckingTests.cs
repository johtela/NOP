namespace NOP.Testbench
{
	using System;
	using NOP;
	using NOP.Collections;
	
	public class TypeCheckingTests
	{
		[Test]
		public void TestSimpleExpression ()
		{
			var expr = TypeExpr.Lit ("foo");
			Check.AreEqual (expr.GetExprType (TypeEnv.Empty), new MonoType.Var ("System.String"));
		}
	}
}