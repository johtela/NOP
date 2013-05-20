namespace NOP.Grammar
{
	using System;
	using Collections;
	using Grammar;
	using Parsing;
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
			return SExprParser.Expr ().Parse (Input.FromSExpr (sexp));
		}

		public class _Application : Expression
		{
			public readonly Expression Function;
			public readonly StrictList<Expression> Parameters;

			public _Application (SExpr sexp, Expression function,
				StrictList<Expression> parameters) : base (sexp)
			{
				Function = function;
				Parameters = parameters;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				var te = Function.GetTypeExpr ();

				if (Parameters.IsEmpty)
					return TypeExpr.Builder.App (te, null);

				for (var pars = Parameters; !pars.IsEmpty; pars = pars.Rest)
					te = TypeExpr.Builder.App (te, pars.First.GetTypeExpr ());
				return te;
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (Function);
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
				Expression elseExpression) : base (sexp)
			{
				Condition = condition;
				ThenExpression = thenExpression;
				ElseExpression = elseExpression;
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
			public readonly StrictList<VariableDefinition> Parameters;
			public readonly Expression FunctionBody;

			public _Lambda (SExpr sexp, StrictList<VariableDefinition> parameters, Expression functionBody)
				: base (sexp)
			{
				Parameters = parameters;
				FunctionBody = functionBody;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.MultiLam (Parameters.Map (p => p.Name.Symbol.Name),
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
			public readonly VariableDefinition Variable;
			public readonly Expression Value;
			public readonly Expression Body;

			public _Let (SExpr sexp, VariableDefinition variable, Expression value, Expression body)
				: base (sexp)
			{
				Variable = variable;
				Value = value;
				Body = body;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Let (Variable.Name.Symbol.Name, Value.GetTypeExpr (),
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
			public readonly new SExpr.Literal Literal;

			public _Literal (SExpr.Literal literal) : base (literal)
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

			public _Quoted (SExpr sexp, Expression quotedExpression) : base (sexp)
			{
				QuotedExpression = quotedExpression;
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

			public _Set (SExpr sexp, _Symbol variable, Expression value) : base (sexp)
			{
				Variable = variable;
				Value = value;
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
			public readonly new SExpr.Symbol Symbol;

			public _Symbol (SExpr.Symbol symbol) : base (symbol)
			{
				Symbol = symbol;
			}

			public override TypeExpr GetTypeExpr ()
			{
				base.GetTypeExpr ();
				return TypeExpr.Builder.Var (Symbol.Name);
			}
		}

		public static Expression Application (SExpr sexp, Expression function,
				StrictList<Expression> parameters)
		{
			return new _Application (sexp, function, parameters);
		}

		public static Expression If (SExpr sexp, Expression condition, Expression thenExpression,
			Expression elseExpression)
		{
			return new _If (sexp, condition, thenExpression, elseExpression);
		}

		public static Expression Lambda (SExpr sexp, StrictList<VariableDefinition> parameters,
			Expression functionBody)
		{
			return new _Lambda (sexp, parameters, functionBody);
		}

		public static Expression Let (SExpr sexp, VariableDefinition variable, Expression value, 
			Expression body)
		{
			return new _Let (sexp, variable, value, body);
		}

		public static Expression Literal (SExpr.Literal literal)
		{
			return new _Literal (literal);
		}

		public static Expression Quoted (SExpr sexp, Expression quotedExpression)
		{
			return new _Quoted (sexp, quotedExpression);
		}

		public static Expression Set (SExpr sexp, _Symbol variable, Expression value)
		{
			return new _Set (sexp, variable, value);
		}

		public static Expression Symbol (SExpr.Symbol symbol)
		{
			return new _Symbol (symbol);
		}
	}
}