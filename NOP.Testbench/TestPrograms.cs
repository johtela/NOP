namespace NOP.Testbench
{
	using NOP.Testing;
	using System.Web.Script.Serialization;

	public class TestPrograms : ExprUser
	{
		public static ExprBuilder SimpleLet ()
		{
			return LetIn (Let ("x", A (33)), S ("x"));
		}

		public static ExprBuilder SimpleLambda ()
		{
			return LetIn (Let ("getFoo", Lambda (P (), A ("foo"))), Call ("getFoo"));
		}

		public static ExprBuilder SimpleIf ()
		{
			return If (A (false), A ("foo"), A ("bar"));
		}

		public static ExprBuilder ComplexIf ()
		{
			return
				LetIn (Let ("foo", Lambda (P ("i"),
					If (Call ("eq?", S ("i"), A (3)),
						A ("That's numberwang!"),
						A ("That's a number")))),
				Call ("foo", A (3)));
		}

		public static ExprBuilder NestedLets ()
		{
			return
				LetIn (Let ("foo", A (42)),
				LetIn (Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y")))),
				Call ("bar", S ("foo"), A (3))));
		}

		public static ExprBuilder NestedLambdas ()
		{
			return Call (
				Lambda (P ("foo"),
					LetIn (Let ("bar", Lambda (P ("x", "y"), Call ("eq?", S ("x"), S ("y")))),
					Call ("bar", S ("foo"), A (3)))
			), A (4));
		}

		public static ExprBuilder SimpleModule ()
		{
			return Mod ("test",
				Def ("foo", T ("Int32"), A (42)));
		}

		public static ExprBuilder RecursiveFunction ()
		{
			return LetRec (And (Let ("member", Lambda (P ("list", "item"),
				If (Call ("eq?", Call ("first", S ("list")), S ("item")),
					A (true), Call ("member", Call ("rest", S ("list")), S ("item")))))),
				S ("member"));
		}
	}
}
