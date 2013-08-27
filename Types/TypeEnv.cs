namespace NOP
{
	using System;
	using System.Linq;
	using Collections;
	using System.Collections.Generic;
	using System.Reflection;
	
	/// <summary>
	/// The type environment maps expression variables (NOT type variables) to polytypes.
	/// It is used to track the symbols in the expression to be checked and their types.
	/// </summary>
	public class TypeEnv
	{
		private readonly Map<Name, Polytype> _map;
			
		private TypeEnv (Map<Name, Polytype> map)
		{
			_map = map;
		}
		
		/// <summary>
		/// Returns type of variable with the given name.
		/// </summary>
		public Polytype Find (Name name)
		{
			return _map [name];
		}
			
		/// <summary>
		/// Check whether a type environment has a variable with the specified name defined.
		/// </summary>
		public bool Contains (Name name)
		{
			return _map.Contains (name);
		}
			
		/// <summary>
		/// Add a variable with the given type to the enironment.
		/// </summary>
		public TypeEnv Add (Name name, Polytype type)
		{
			return new TypeEnv (_map.Add (name, type));
		}
			
		/// <summary>
		/// Returns all the type variables defined in the environment.
		/// </summary>
		public Set<string> GetTypeVars ()
		{
			return _map.Values.Aggregate (Set<string>.Empty, (s, pt) => s + pt.GetTypeVars ());
		}
		
		/// <summary>
		/// Initial type environment.
		/// </summary>
		public static TypeEnv Initial
		{
			get
			{ 
				return InitialEnvironment.Value;
			}
		}
		
		/// <summary>
		/// Private helper class for defining the initial environment.
		/// </summary>
		private class InitialEnvironment : MonoType.Builder
		{
			private static IEnumerable<string> GetTypeVars (MonoType[] parameters)
			{
				foreach (var par in parameters)
					if (par is MonoType.Var) yield return (par as MonoType.Var).Name;
			}

			private static Polytype Pt (MonoType result, params MonoType[] parameters)
			{
				return new Polytype (Lambda (List.Create (parameters), result), GetTypeVars (parameters));
			}

			private static Tuple<Name, Polytype> Ft (string funcName, Polytype pt)
			{
				return Tuple.Create (new Name(funcName), pt);
			}

			public static readonly TypeEnv Value =
				new TypeEnv (Map<Name, Polytype>.FromPairs (
					Ft ("set!", Pt (Constant (new Name ("Void", "System")), Variable ("a"), Variable ("a"))),
					Ft ("eq?", Pt (Constant (new Name ("Boolean", "System")), Variable ("a"), Variable ("a")))
				));

			private static IEnumerable<Tuple<Name, Polytype>> TypesInAssembly (Assembly assy)
			{
				return (from t in assy.GetTypes ()
						let ns = Namespace.Get (t.Namespace)
						select Members (t, ns).Prepend (Tuple.Create (new Name (t.Name, ns), TypeToPolytype (t))))
					   .Collect ();
			}

			private static IEnumerable<Tuple<Name, Polytype>> Member (MemberInfo mi, Namespace ns)
			{
				var name = new Name (mi.Name, ns);
				if (mi is MethodInfo)
					return Tuple.Create (name, MethodToPolytype (mi as MethodInfo)).AsEnumerable ();
				if (mi is FieldInfo)
					return Tuple.Create (name, TypeToPolytype ((mi as FieldInfo).FieldType)).AsEnumerable ();
				if (mi is PropertyInfo)
					return Tuple.Create (name, PropertyToPolytype (mi as PropertyInfo)).AsEnumerable ();
				if (mi is Type)
				{
					var type = mi as Type;
					return Members (type, ns + type.Name).Prepend (Tuple.Create (name, TypeToPolytype (type)));
				}
				throw new ArgumentException ("Unknown member type", mi.GetType ().Name);
			}

			private static IEnumerable<Tuple<Name, Polytype>> Members (Type type, Namespace ns)
			{
				return (from m in type.GetMembers ()
						select Member (m, ns)).Collect ();
			}

			private static MonoType TypeToMonoType (Type type)
			{
				return type.IsGenericParameter ?
					Variable (type.Name) :
					Constant (new Name (type.Name, type.Namespace));
			}

			private static Polytype TypeToPolytype (Type type)
			{
				var mt = TypeToMonoType (type);
				if (type.IsGenericTypeDefinition)
					return new Polytype (mt, type.GetGenericArguments ().Select (t => t.Name));
				else
					return new Polytype (mt);
			}

			private static Polytype MethodToPolytype (MethodInfo mi)
			{
				var pars = List.FromEnumerable (from pi in mi.GetParameters ()
												select TypeToMonoType (pi.ParameterType));
				var mt = Lambda (pars, TypeToMonoType (mi.ReturnType));
				if (mi.IsGenericMethodDefinition)
					return new Polytype (mt, mi.GetGenericArguments ().Select (t => t.Name));
				else
					return new Polytype (mt);
			}

			private static Polytype PropertyToPolytype (PropertyInfo pi)
			{
				var pars = List.FromEnumerable (from par in pi.GetIndexParameters ()
												select TypeToMonoType (par.ParameterType));
				return new Polytype (Lambda (pars, TypeToMonoType (pi.PropertyType)));
			}
		}
	}
}