namespace NOP.Grammar
{
	using System;
	using NOP.Collections;

	public abstract class TypeReference : AstNode
	{
		public TypeReference (SExpr sexp) : base (sexp)
		{
		}

		public class _Simple : TypeReference
		{
			public readonly Expression._Symbol TypeName;

			public _Simple (Expression._Symbol typeName)
				: base (typeName.SExp)
			{
				TypeName = typeName;
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (TypeName);
			}
		}

		public class _Generic : TypeReference
		{
			public readonly Expression._Symbol TypeName;
			public readonly StrictList<TypeReference> TypeParams;

			public _Generic (SExpr sexp, Expression._Symbol typeName,
				StrictList<TypeReference> typeParams)
				: base (sexp)
			{
				TypeName = typeName;
				TypeParams = typeParams;
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (TypeName);
				TypeParams.Foreach (action);
			}
		}

		public class _Function : TypeReference
		{
			public readonly TypeReference ArgumentType, ResultType;

			public _Function (SExpr sexp, TypeReference argumentType,
				TypeReference resultType)
				: base (sexp)
			{
				ArgumentType = argumentType;
				ResultType = resultType;
			}

			protected override void DoForChildNodes (Action<AstNode> action)
			{
				action (ArgumentType);
				action (ResultType);
			}
		}

		public static TypeReference Simple (Expression._Symbol typeName)
		{
			return new _Simple (typeName);
		}

		public static TypeReference Generic (SExpr sexp, Expression._Symbol typeName,
			StrictList<TypeReference> typeParams)
		{
			return new _Generic (sexp, typeName, typeParams);
		}

		public static TypeReference Function (SExpr sexp, TypeReference argumentType,
			TypeReference resultType)
		{
			return new _Function (sexp, argumentType, resultType);
		}
	}
}

