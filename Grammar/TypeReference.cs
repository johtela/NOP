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

			public override void VisitNodes (Action<AstNode> visitor)
			{
				TypeName.VisitNodes (visitor);
				base.VisitNodes (visitor);
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

			public override void VisitNodes (Action<AstNode> visitor)
			{
				TypeName.VisitNodes (visitor);
				TypeParams.Foreach (n => n.VisitNodes (visitor));
				base.VisitNodes (visitor);
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

			public override void VisitNodes (Action<AstNode> visitor)
			{
				ArgumentType.VisitNodes (visitor);
				ResultType.VisitNodes (visitor);
				base.VisitNodes (visitor);
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

