namespace NOP.CodeGen
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Linq;
	using System.Text;

	public delegate T CGen<T> () where T : Emit;

	public static class Gen
	{
		public static CGen<T> ToCGen<T> (this T ctx) where T : Emit
		{
			return () => ctx;
		}

		public static CGen<U> Bind<T, U> (this CGen<T> gen, Func<T, CGen<U>> func)
			where T : Emit where U : Emit
		{
			return func (gen ());
		}

		public static CGen<T> Do<T> (this CGen<T> gen, Action<T> action) where T : Emit
		{
			return gen.Bind (t =>
			{
				action (t);
				return gen;
			});
		}

		public static CGen<U> Select<T, U> (this CGen<T> gen, Func<T, U> select)
			where T : Emit where U : Emit
		{
			return gen.Bind (x => select (x).ToCGen<U> ());
		}

		public static CGen<V> SelectMany<T, U, V> (this CGen<T> gen, Func<T, CGen<U>> project, 
			Func<T, U, V> select) where T : Emit where U : Emit where V : Emit
		{
			return gen.Bind (x => project (x).Bind (y => select (x, y).ToCGen<V> ()));
		}


		public static CGen<Emit.Type> Type (this CGen<Emit.Assembly> assembly, string name)
		{
			return from a in assembly 
				   select new Emit.Type (a, a.ModuleBuilder.DefineType (name));
		}

		public static CGen<Emit.Method> Method (this CGen<Emit.Type> type, string name)
		{
			return from t in type
				   select new Emit.Method (t, t.TypeBuilder.DefineMethod (name, MethodAttributes.Public));
		}

		public static CGen<Emit.MethodBody> MethodBody (this CGen<Emit.Method> method)
		{
			return from m in method
				   select new Emit.MethodBody (m, m.MethodBuilder.GetILGenerator ());
		}

		public static CGen<Emit.Type> Field (this CGen<Emit.Type> type, string name, Type fieldType)
		{
			return type.Do (t => t.TypeBuilder.DefineField (name, fieldType, FieldAttributes.Public));
		}
	}
}
