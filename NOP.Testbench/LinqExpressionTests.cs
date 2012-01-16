using System;
using System.Linq.Expressions;
using LExpr = System.Linq.Expressions.Expression;

namespace NOP.Testbench
{
	public class LinqExpressionTests : ExprUser
	{
		[Test]
		public void TestVoidMethod ()
		{
			var mi = typeof(Console).GetMethod ("Clear");
			BlockExpression block = LExpr.Block (LExpr.Call (mi), LExpr.Constant (null, typeof(object)));
			
			Expression<Func<object >> lambda = LExpr.Lambda<Func<object>> (block);
			Func<object >  fun = lambda.Compile ();
			
			fun ();
		}
	}
	
	public class Foo
	{
		public static int Bar ()
		{
			return 42;
		}
	}
}

