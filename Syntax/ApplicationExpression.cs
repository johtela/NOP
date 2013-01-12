namespace NOP
{
	using System;
    using System.Linq;
	using Collections;
    using System.Collections.Generic;
	
	public class ApplicationExpression : Expression
	{
		public readonly Expression FuncName;
		public readonly NOPList<Expression> Parameters;
		
		public ApplicationExpression (SExpr.List funcExpr) : base (funcExpr)
		{
			var sexps = funcExpr.Items;
			FuncName = Parse (Expect<SExpr> (ref sexps, "function name or lambda expression"));
			Parameters = sexps.Map (sexp => Parse (sexp));
		}
		
		public override TypeExpr GetTypeExpr ()
		{
			base.GetTypeExpr ();
			var te = FuncName.GetTypeExpr ();
			
			if (Parameters.IsEmpty)
				return TypeExpr.Builder.App (te, null);
			
			for (var pars = Parameters; !pars.IsEmpty; pars = pars.Rest)
				te = TypeExpr.Builder.App (te, pars.First.GetTypeExpr ());
			return te;
		}

        protected override IEnumerable<AstNode> GetChildNodes ()
        {
            return FuncName | Parameters;
        }
	}
}