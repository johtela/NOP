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
		public Bindings TypeEnvironment;
		public Substitution Subs;
		public MonoType Type;

		public Expression (SExpr sexp) : base (sexp)
		{ }

		protected abstract TypeCheck GetTypeCheck ();

		/// <summary>
		/// Get the type expression that is used to type check this expression.
		/// </summary>
		public TypeCheck TypeCheck ()
		{
			return TC.DoAfter (GetTypeCheck (), 
				(st, exp) => 
				{
					TypeEnvironment = st.Env;
					Subs = st.Subs;
					Type = exp;
				});
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

			protected override TypeCheck GetTypeCheck ()
			{
				var tc = Function.TypeCheck ();

				if (Parameters.IsEmpty)
					return TC.Application (tc, null);

				for (var pars = Parameters; !pars.IsEmpty; pars = pars.Rest)
					tc = TC.Application (tc, pars.First.TypeCheck ());
				return tc;
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return Function.LeftConcat (Parameters.LeftCast<Expression, AstNode> ().LeftRecurse ());
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

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.IfElse (Condition.TypeCheck (), ThenExpression.TypeCheck (),
					ElseExpression.TypeCheck ());
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return Condition.LeftConcat (ThenExpression).LeftConcat (ElseExpression);
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

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.MultiLambda (Parameters.Map (p => p.Name.Symbol.Name),
					FunctionBody.TypeCheck ());
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return (Parameters.LeftCast<VariableDefinition, AstNode> ().LeftRecurse ().LeftConcat (FunctionBody));
			}

			protected override Visual GetVisual ()
			{
				var sexps = ((SExpr.List)SExp).Items;
				var slambda = sexps.First;
				var sparams = sexps.RestL.First;
				var sbody = sexps.RestL.RestL.First;

				return V.Indented (
					V.HStack (VAlign.Top, V.Depiction (slambda), V.Depiction (sparams)),
					V.Depiction (sbody), 24);
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

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.LetIn (Variable.Name.Symbol.Name, Value.TypeCheck (),
					Body.TypeCheck ());
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return Variable.LeftConcat (Value).LeftConcat (Body);
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

		public class _LetRec : Expression
		{
			public readonly StrictList<Tuple<VariableDefinition, Expression>> Definitions;
			public readonly Expression Body;

			public _LetRec (SExpr sexp, StrictList<Tuple<VariableDefinition, Expression>> definitions, 
				Expression body) : base (sexp) 
			{
				Definitions = definitions;
			}

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.LetRec (Definitions.Map(
					t => Tuple.Create(t.Item1.Name.Symbol.Name, t.Item2.TypeCheck ())),
					Body.GetTypeCheck ());
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return (ILeftReducible<AstNode>)Definitions.Collect<ILeftReducible<AstNode>> (
					t => List.Create<ILeftReducible<AstNode>>(t.Item1, t.Item2));
			}

		}

		public class _Literal : Expression
		{
			public readonly new SExpr.Literal Literal;

			public _Literal (SExpr.Literal literal) : base (literal)
			{
				Literal = literal;
			}

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.Literal (Literal.Value);
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

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.Literal (SExp);
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return QuotedExpression;
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

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.Application (
					TC.Application (TC.Variable ("set!"), Variable.TypeCheck ()),
					Value.TypeCheck ());
			}

			protected override ILeftReducible<AstNode> AsReducible ()
			{
				return Variable.LeftConcat (Value);
			}
		}

		public class _Symbol : Expression
		{
			public readonly new SExpr.Symbol Symbol;

			public _Symbol (SExpr.Symbol symbol) : base (symbol)
			{
				Symbol = symbol;
			}

			protected override TypeCheck GetTypeCheck ()
			{
				return TC.Variable (Symbol.Name);
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

		public static Expression LetRec (SExpr sexp, VariableDefinition variable, Expression value,
			Expression body)
		{
			return null; // new _LetRec (sexp, variable, value, body);
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