namespace NOP.CodeGen
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Text;

	public abstract class Emit
	{
		public class Assembly : Emit
		{
			public readonly AssemblyBuilder AssemblyBuilder;
			public readonly ModuleBuilder ModuleBuilder;

			public Assembly (AssemblyBuilder ab, ModuleBuilder mb)
			{
				AssemblyBuilder = ab;
				ModuleBuilder = mb;
			}
		}

		public class Type : Assembly
		{
			public readonly TypeBuilder TypeBuilder;

			public Type (Assembly assy, TypeBuilder tb) :
				base (assy.AssemblyBuilder, assy.ModuleBuilder)
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

		public Type AsType
		{
			get { return (Type)this; }
		}

		public Method AsMethod
		{
			get { return (Method)this; }
		}

		public MethodBody AsMethodBody
		{
			get { return (MethodBody)this; }
		}
	}
}