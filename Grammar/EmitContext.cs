namespace NOP.CodeGen
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Text;

	public class EmitContext
	{
		public readonly AssemblyBuilder AssemblyBuilder;
		public readonly ModuleBuilder ModuleBuilder;

		public EmitContext (AssemblyBuilder ab, ModuleBuilder mb)
		{
			AssemblyBuilder = ab;
			ModuleBuilder = mb;
		}

		public class Type : EmitContext
		{
			public readonly TypeBuilder TypeBuilder;

			public Type (EmitContext ctx, TypeBuilder tb) :
				base (ctx.AssemblyBuilder, ctx.ModuleBuilder)
			{
				TypeBuilder = tb;
			}
		}

		public class Method : Type
		{
			public readonly MethodBase MethodBase;

			public Method (Type type, MethodBase mb) :
				base (type, type.TypeBuilder)
			{
				MethodBase = mb;
			}

			public bool IsConstructor
			{
				get { return MethodBase is ConstructorInfo; }
			}

			public ConstructorBuilder ConstructorBuilder
			{
				get { return (ConstructorBuilder)MethodBase; }
			}

			public MethodBuilder MethodBuilder
			{
				get { return (MethodBuilder)MethodBase; }
			}
		}

		public class MethodBody : Method
		{
			public readonly ILGenerator ILGenerator;

			public MethodBody (Method m, ILGenerator ig) :
				base (m, m.MethodBase)
			{
				ILGenerator = ig;
			}
		}
	}
}
