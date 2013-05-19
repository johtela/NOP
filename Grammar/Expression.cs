namespace NOP.Grammar
{
	using System;
	using Collections;
	using Visuals;
	using V = NOP.Visuals.Visual;

	/// <summary>
	/// Abstract class representing any language expression. The root class of the 
	/// abstract syntax tree.
	/// </summary>
	public abstract class Expression : AstNode
	{
		public Expression (SExpr sexp) : base (sexp)
		{ }
		
		/// <summary>
		/// Get the type expression that is used to type check this expression.
		/// </summary>
		public virtual TypeExpr GetTypeExpr ()
		{
			TypeExpr.CurrentExpression = this;
			return null;
		}
				
		/// <summary>
		/// Parse an S-expression and generate the AST.
		/// </summary>
		public static Expression Parse (SExpr sexp)
		{
			if (sexp is SExpr.Symbol)
				return new _Symbol (sexp as SExpr.Symbol);
			var slist = sexp as SExpr.List;
			if (slist != null)
			{
				var list = (sexp as SExpr.List).Items;
				if (list.IsEmpty)
					return new _Literal (null);
				var symbol = list.First as SExpr.Symbol;
				if (symbol != null)
				{
					// Check if we have any of the special forms as first item.
					switch (symbol.Name)
					{
						case "quote":
							return new _Quoted (slist);
						case "if":
							return new _If (slist);
						case "let":
							return new _Let (slist);
						case "lambda":
							return new _Lambda (slist);
						case "set!":
							return new _Set (slist);
					}
				}
				// Otherwise do a function call.
				return new _Application (slist);
			}
			return new _Literal (sexp as SExpr.Literal);
		}

		public class _Application : Expression
		{
			public readonly Expression FuncName;
			public readonly StrictList<Expression> Parameters;

			public _Application (SExpr sexp, Expression funcName,
				StrictList<Expression> parameters)
				: base (sexp)
			{
				FuncName = funcName;
				Parameters = parameters;
			}

			public _Application (SExpr.List funcExpr)
				: base (funcExpr)
			{
				var sexps = funcExpr.Items;
				FuncName = Parse (Expect<SExpr> (ref sexps, "function name or lambda expression"));
				Parameters = List.MapReducible (sexps, sexp => Parse (sexp));
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

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (FuncName);
				Parameters.Foreach (action);
			}

			protected override Visual GetVisual ()
			{
				var sexps = ((SExpr.List)SExp).Items;

				return V.HStack (VAlign.Top, V.Depiction (sexps.First),
					V.Parenthesize (V.HList (sexps.RestL)));
			}
		}

		public class _If : Expression
		{
			public readonly Expression Condition;
			public readonly Expression ThenExpression;
			public readonly Expression ElseExpression;

			public _If (SExpr sexp, Expression condition, Expression thenExpression,
				Expression elseExpression)
				: base (sexp)
			{
				Condition = condition;
				ThenExpression = thenExpression;
				ElseExpression = elseExpression;
			}

			public _If (SExpr.List ifExpr)
				: base (ifExpr)
			{
				var sexps = ifExpr.Items.RestL;
				Condition = Parse (Expect<SExpr> (ref sexps, "condition"));
				ThenExpression = Parse (Expect<SExpr> (ref sexps, "then expression"));
				ElseExpression = Parse (Expect<SExpr> (ref sexps, "else expression"));
			}

			protected override Visual GetVisual ()
			{
				var skeyword = ((SExpr.List)SExp).Items.First;
				var scond = Condition.SExp;
				var sthen = ThenExpression.SExp;
				var selse = ElseExpression.SExp;

				return V.HStack (VAlign.Top,
					V.Depiction (skeyword), V.Depiction (scond), V.VStack (HAlign.Left,
						V.HStack (VAlign.Top, V.Label ("then"), V.Depiction (sthen)),
						V.HStack (VAlign.Top, V.Label ("else"), V.Depiction (selse)))
				);
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.If (Condition.GetTypeExpr (), ThenExpression.GetTypeExpr (),
											ElseExpression.GetTypeExpr ());
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Condition);
				action (ThenExpression);
				action (ElseExpression);
			}
		}

		public class _Lambda : Expression
		{
			public readonly StrictList<_Symbol> Parameters;
			public readonly Expression FunctionBody;

			public _Lambda (SExpr sexp, StrictList<_Symbol> parameters,
				Expression functionBody)
				: base (sexp)
			{
				Parameters = parameters;
				FunctionBody = functionBody;
			}

			public _Lambda (SExpr.List lambdaExpr)
				: base (lambdaExpr)
			{
				var sexps = lambdaExpr.Items.RestL;
				var pars = Expect<SExpr.List> (ref sexps, "list of parameters");
				Parameters = List.MapReducible (pars.Items, sexp =>
				{
					var par = sexp as SExpr.Symbol;
					if (par == null)
						ParseError (sexp, "Expected a symbol");
					return new _Symbol (par);
				});
				if (sexps.IsEmpty)
					ParseError (lambdaExpr, "Function body is missing");
				FunctionBody = Parse (sexps.First);
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.MultiLam (Parameters.Map (s => s.Symbol.Name),
					FunctionBody.GetTypeExpr ());
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				Parameters.Foreach (action);
				action (FunctionBody);
			}

			protected override Visual GetVisual ()
			{
				var sexps = ((SExpr.List)SExp).Items;
				var slambda = sexps.First;
				var sparams = sexps.RestL.First;
				var sbody = sexps.RestL.RestL.First;

				return V.VStack (HAlign.Left,
					V.HStack (VAlign.Top, V.Frame (V.Depiction (slambda), FrameKind.Rectangle), V.Depiction (sparams)),
					V.Margin (V.Depiction (sbody), 24));
			}
		}

		public class _Let : Expression
		{
			public readonly _Symbol Variable;
			public readonly Expression Value;
			public readonly Expression Body;

			public _Let (SExpr sexp, _Symbol variable, Expression value,
				Expression body)
				: base (sexp)
			{
				Variable = variable;
				Value = value;
				Body = body;
			}

			public _Let (SExpr.List letExpr)
				: base (letExpr)
			{
				var sexps = letExpr.Items.RestL;
				Variable = new _Symbol (Expect<SExpr.Symbol> (ref sexps, "variable"));
				Value = Parse (Expect<SExpr> (ref sexps, "variable value"));
				Body = Parse (Expect<SExpr> (ref sexps, "body of let expression"));
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Let (Variable.Symbol.Name, Value.GetTypeExpr (),
											 Body.GetTypeExpr ());
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Variable);
				action (Value);
				action (Body);
			}

			protected override Visual GetVisual ()
			{
				var slet = ((SExpr.List)SExp).Items.First;
				var svar = Variable.SExp;
				var sval = Value.SExp;
				var sbody = Body.SExp;

				return V.VStack (HAlign.Left,
					V.HStack (VAlign.Top, V.Depiction (slet), V.Depiction (svar),
						V.Label ("="), V.Depiction (sval)),
					V.HStack (VAlign.Top, V.Depiction (sbody)));
			}
		}

		public class _Literal : Expression
		{
			public readonly SExpr.Literal Literal;

			public _Literal (SExpr.Literal literal)
				: base (literal)
			{
				Literal = literal;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Lit (Literal.Value);
			}

			protected override Visual GetVisual ()
			{
				return Literal.Value is string ?
					Visual.Label (string.Format ("\"{0}\"", Literal.Value)) :
					base.GetVisual ();
			}
		}

		public class _Quoted : Expression
		{
			public readonly Expression QuotedExpression;

			public _Quoted (SExpr sexp, Expression quotedExpression)
				: base (sexp)
			{
				QuotedExpression = quotedExpression;
			}

			public _Quoted (SExpr.List quoteSExp)
				: base (quoteSExp)
			{
				var sexps = quoteSExp.Items.RestL;
				QuotedExpression = Parse (Expect<SExpr> (ref sexps, "quoted expression"));
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Lit (SExp);
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (QuotedExpression);
			}
		}
		public class _Set : Expression
		{
			public readonly _Symbol Variable;
			public readonly Expression Value;

			public _Set (SExpr sexp, _Symbol variable, Expression value)
				: base (sexp)
			{
				Variable = variable;
				Value = value;
			}

			public _Set (SExpr.List setExpr)
				: base (setExpr)
			{
				var sexps = setExpr.Items.RestL;
				Variable = new _Symbol (Expect<SExpr.Symbol> (ref sexps, "variable"));
				Value = Parse (Expect<SExpr> (ref sexps, "right hand side of set! clause"));
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.App (
					TypeExpr.Builder.App (TypeExpr.Builder.Var ("set!"), Variable.GetTypeExpr ()),
					Value.GetTypeExpr ());
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Variable);
				action (Value);
			}
		}

		public class _Symbol : Expression
		{
			public readonly SExpr.Symbol Symbol;

			public _Symbol (SExpr.Symbol symbol)
				: base (symbol)
			{
				Symbol = symbol;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Var (Symbol.Name);
			}
		}
	}
}