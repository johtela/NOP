namespace NOP
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Collections;
	using System.Reflection;

	public class BindingException : Exception
	{
		public BindingException (string msg, params object[] args) 
			: base (string.Format(msg, args)) {}
	}

	public class Bindings
	{
		private readonly Map<string, Polytype> _types;
		private readonly Map<string, Polytype> _definitions;

		public Bindings (Map<string, Polytype> types, Map<string, Polytype> defs)
		{
			_types = types;
			_definitions = defs;
		}

		/// <summary>
		/// Returns type of binding with the given name. Selector specifies whether types or 
		/// definitions are searched.
		/// </summary>
		public Polytype FindName (string name, Func<Bindings, Map<string, Polytype>> selector)
		{
			var res = selector (this).TryGetValue (name);
			if (!res)
				throw new BindingException ("Named binding '{0}' not found.", name);
			return res;
		}

		public Polytype FindType (string name)
		{
			return FindName (name, b => b._types);
		}

		public Polytype FindDefinition (string name)
		{
			return FindName (name, b => b._definitions);
		}

		/// <summary>
		/// Add a new type with the given name to the environment.
		/// </summary>
		public Bindings AddType (string name, Polytype type)
		{
			return new Bindings (_types.Add (name, type), _definitions);
		}

		/// <summary>
		/// Add a definition with the given name and type to the environment.
		/// </summary>
		public Bindings AddDefinition (string name, Polytype type)
		{
			return new Bindings (_types, _definitions.Add (name, type));
		}

		/// <summary>
		/// Replace a definition with the given name with a new type.
		/// </summary>
		public Bindings ReplaceDefinition (string name, Polytype type)
		{
			return new Bindings (_types, _definitions.Replace (name, type));
		}

		/// <summary>
		/// Returns all the type variables defined in the environment.
		/// </summary>
		public Set<string> GetTypeVars ()
		{
			return _definitions.Values.Aggregate (Set<string>.Empty, (s, pt) => s + pt.GetTypeVars ());
		}

		public override string ToString ()
		{
			var res = _types.ReduceLeft (new StringBuilder ("Types: "),
				(sb, t) => sb.AppendLine (string.Format ("{0} :: {1}", t.Item1, t.Item2)));
			return _definitions.ReduceLeft (res.AppendLine ("Definitions: "),
				(sb, t) => sb.AppendLine (string.Format ("{0} :: {1}", t.Item1, t.Item2))).ToString ();
		}

		/// <summary>
		/// Initial type environment.
		/// </summary>
		public static Bindings Initial
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
				return parameters.ReduceLeft (Set<string>.Empty, (s, p) => s + p.GetTypeVars ());
			}

			private static Polytype Pt (MemberInfo mi, MonoType result, params MonoType[] parameters)
			{
				return new Polytype (Lambda (List.Create (parameters), result, mi), GetTypeVars (parameters));
			}

			private static Tuple<string, Polytype> It (string name)
			{
				return Tuple.Create (name, new Polytype (Constant (name)));
			}

			private static Tuple<string, Polytype> It (Type type)
			{
				var mt = FromType (type);
				return Tuple.Create (CleanupName (type.Name), new Polytype (mt, mt.GetTypeVars ()));
			}

			private static Tuple<string, Polytype> Ft (string funcName, Polytype pt)
			{
				return Tuple.Create (funcName, pt);
			}

			private static string CleanupName (string name)
			{
				var i = name.IndexOf ('`');
				return i >= 0 ? name.Remove (i) : name;
			}

			private static MonoType FromType (Type type)
			{
				var name = CleanupName (type.Name);
				return type.IsGenericParameter ?
					Variable (name) :
					type.HasElementType ?
						FromType (type.GetElementType ()) :
						type.ContainsGenericParameters ?
							Constant (name, type.GetGenericArguments ().Select (t => FromType (t)), type) :
							Constant (name, null, type);
			}

			private static Polytype Met (Type hostType, string method)
			{
				var mi = hostType.GetMethod (method);
				var ret = FromType (mi.ReturnType);
				var pars = (from p in mi.GetParameters ()
							select FromType (p.ParameterType));
				if (!mi.IsStatic)
					pars = pars.Prepend (FromType (hostType));
				return Pt (mi, ret, pars.ToArray ());
			}

			private static Polytype Prop (Type hostType, string property)
			{
				var pi = hostType.GetProperty (property);
				var ret = FromType (pi.PropertyType);
				return Pt (pi, ret, FromType (hostType));
			}

			private static Bindings Generate ()
			{
				var Prelude = typeof (Prelude);
				var StrictList = typeof (StrictList<>);
				var List = typeof (List);

				return new Bindings (
					Map<string, Polytype>.FromPairs (
						It ("Void"), It (typeof (Byte)), It (typeof (SByte)), It (typeof (Int16)), It (typeof (UInt16)),
						It (typeof (Int32)), It (typeof (UInt32)), It (typeof (Int64)), It (typeof (UInt64)),
						It (typeof (Single)), It (typeof (Double)), It (typeof (Char)), It (typeof (Boolean)),
						It (typeof (Object)), It (typeof (String)), It (typeof (Decimal)), It (typeof (StrictList<>))),
					Map<string, Polytype>.FromPairs (
						Ft ("set!", Met (Prelude, "Set")),
						Ft ("eq?", Met (Prelude, "Eq")),
						Ft ("first", Prop (StrictList, "First")),
						Ft ("rest", Prop (StrictList, "Rest"))
				));
			}

			public static readonly Bindings Value = Generate ();
		}
	}
}
