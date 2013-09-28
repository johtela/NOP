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

			public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
			{
				return func (TypeName.ReduceLeft (acc, func), this);
			}

			public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
			{
				return TypeName.ReduceRight (func, func (this, acc));
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

			public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
			{
				return func (TypeParams.ReduceLeft (TypeName.ReduceLeft (acc, func), func), this);
			}

			public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
			{
				return TypeParams.ReduceRight (func, TypeName.ReduceRight (func, func (this, acc)));
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

			public override U ReduceLeft<U> (U acc, Func<U, AstNode, U> func)
			{
				return func (ResultType.ReduceLeft (ArgumentType.ReduceLeft(acc, func), func), this);
			}

			public override U ReduceRight<U> (Func<AstNode, U, U> func, U acc)
			{
				return ResultType.ReduceRight (func, ArgumentType.ReduceRight (func, func (this, acc)));
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

