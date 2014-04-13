namespace NOP
{
	using System;
	using System.Linq;
	using Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text;
	
	/// <summary>
	/// The type environment maps expression variables (NOT type variables) to polytypes.
	/// It is used to track the symbols in the expression to be checked and their types.
	/// </summary>
	public class TypeEnv
	{
		private readonly Map<string, Polytype> _map;
			
		private TypeEnv (Map<string, Polytype> map)
		{
			_map = map;
		}
		
		/// <summary>
		/// Returns type of variable with the given name.
		/// </summary>
		public Polytype Find (string name)
		{
			return _map [name];
		}
			
		/// <summary>
		/// Check whether a type environment has a variable with the specified name defined.
		/// </summary>
		public bool Contains (string name)
		{
			return _map.Contains (name);
		}
			
		/// <summary>
		/// Add a variable with the given type to the environment.
		/// </summary>
		public TypeEnv Add (string name, Polytype type)
		{
			return new TypeEnv (_map.Add (name, type));
		}

		/// <summary>
		/// Replace a variable with the new type.
		/// </summary>
		public TypeEnv Replace (string name, Polytype type)
		{
			return new TypeEnv (_map.Replace (name, type));
		}

		/// <summary>
		/// Returns all the type variables defined in the environment.
		/// </summary>
		public Set<string> GetTypeVars ()
		{
			return _map.Values.Aggregate (Set<string>.Empty, (s, pt) => s + pt.GetTypeVars ());
		}

		public override string ToString ()
		{
			return _map.ReduceLeft (new StringBuilder (),
				(sb, t) => sb.AppendLine (string.Format ("{0} :: {1}", t.Item1, t.Item2)))
				.ToString ();
		}
		
		/// <summary>
		/// Initial type environment.
		/// </summary>
		public static TypeEnv Initial
		{
			get { return InitialEnvironment.Value; }
		}
		
		/// <summary>
		/// Private helper class for defining the initial environment.
		/// </summary>
		private class InitialEnvironment : MonoType.Builder
		{
			private static IEnumerable<string> GetTypeVars (MonoType[] parameters)
			{
				return from par in parameters
					   where par is MonoType.Var
					   select (par as MonoType.Var).Name;
			}

			private static Polytype Pt (MemberInfo mi, MonoType result, params MonoType[] parameters)
			{
				return new Polytype (Lambda (List.Create (parameters), result), mi, GetTypeVars (parameters));
			}

			private static Tuple<string, Polytype> It (string name)
			{
				return Tuple.Create (name, new Polytype (Constant (name)));
			}

			private static Tuple<string, Polytype> It (Type type)
			{
				var mt = FromType(type);
				return Tuple.Create (type.Name, new Polytype (mt, type, mt.GetTypeVars ()));
			}

			private static Tuple<string, Polytype> Ft (string funcName, Polytype pt)
			{
				return Tuple.Create (funcName, pt);
			}

			private static MonoType FromType (Type type)
			{
				return type.IsGenericParameter ?
					Variable (type.Name) :
					type.HasElementType ?
						FromType (type.GetElementType ()) :
						type.ContainsGenericParameters ?
							Constant (type.Name, type.GetGenericArguments ().Select (t => FromType (t))) :
							Constant (type.Name);
			}

			private static Polytype Met (Type hostType, string method)
			{
				var mi = hostType.GetMethod (method);
				var ret = FromType (mi.ReturnType);
				var pars = (from p in mi.GetParameters ()
						   select FromType (p.ParameterType)).ToArray ();
				return Pt (mi, ret, pars);
			}

			private static TypeEnv Generate ()
			{
				var Prelude = typeof (Prelude);

				return new TypeEnv (Map<string, Polytype>.FromPairs (
					It ("Void"), It (typeof (Byte)), It (typeof (SByte)), It (typeof (Int16)), It (typeof (UInt16)),
					It (typeof (Int32)), It (typeof (UInt32)), It (typeof (Int64)), It (typeof (UInt64)),
					It (typeof (Single)), It (typeof (Double)), It (typeof (Char)), It (typeof (Boolean)),
					It (typeof (Object)), It (typeof (String)), It (typeof (Decimal)), It (typeof (StrictList<>)),
					Ft ("set!", Met (Prelude, "Set")),
					Ft ("eq?", Met (Prelude, "Eq"))
				));
			}

			public static readonly TypeEnv Value = Generate ();
		}
	}
}