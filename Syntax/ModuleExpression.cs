namespace NOP
{
	using System;
	using NOP.Collections;

	public class ModuleExpression : Expression
	{
		public readonly SymbolExpression Name;
		public readonly List<DefinitionExpression> Members;
		
//		public LetExpression (SExpr.List letExpr) : base (letExpr)
//		{
//			var sexps = letExpr.Items.Rest;
//			Variable = new SymbolExpression (Expect<SExpr.Symbol> (ref sexps, "variable"));
//			Value = Parse (Expect<SExpr> (ref sexps, "variable value"));
//			Body = Parse (Expect<SExpr> (ref sexps, "body of let expression"));		
//		}
		
		public ModuleExpression (SExpr.List classExpr) : base (classExpr)
		{

		}
	}
}

