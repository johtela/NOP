namespace NOP
{
	using System;
	
	public class DefineExpression : ListExpression
	{
		public readonly SymbolExpression Lhs;
		public readonly Expression Rhs;
		
		public DefineExpression (SExpr.List defineExpr) : base (defineExpr)
		{
			var sexps = defineExpr.Items.Rest;
			Lhs = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "symbol"));
			Rhs = Parse (Expect<SExpr> (ref sexps, "right hand side of definition clause"));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			throw new NotImplementedException ();
		}
	}
}

