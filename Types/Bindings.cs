namespace NOP
{
	using System;
	using System.Linq;
	using System.Text;
	using Collections;

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
			return _definitions.ReduceLeft (new StringBuilder (),
				(sb, t) => sb.AppendLine (string.Format ("{0} :: {1}", t.Item1, t.Item2)))
				.ToString ();
		}
	}
}
