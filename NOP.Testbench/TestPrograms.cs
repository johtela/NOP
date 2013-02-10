namespace NOP.Testbench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class TestPrograms : ExprUser
	{
		public static ExprBuilder SimpleLet ()
		{
			return Let ("x", A (33), S ("x"));
		}

		public static ExprBuilder SimpleLambda ()
		{
			return Let ("getFoo", Lambda (P (), A ("foo")), Call ("getFoo"));
		}

		public static ExprBuilder SimpleIf ()
		{
			return If (A (false), A ("foo"), A ("bar"));
		}

		public static ExprBuilder ComplexIf ()
		{
			return
				Let ("foo", Lambda (P ("i"),
				If (Call ("eq?", S ("i"), A (3)),
					A ("It's numberwang!"),
					A ("It's a number"))),
				Call ("foo", A (3)));
		}

		public static ExprBuilder NestedLets ()
		{
			return
				Let ("foo", A (42),
				Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y"))),
				Call ("bar", S ("foo"), A (3))));
		}

		public static ExprBuilder NestedLambdas ()
		{
			return Call (
				Lambda (P ("foo"),
					Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y"))),
					Call ("bar", S ("foo"), A (3)))
			), A (4));
		}
	}
}
