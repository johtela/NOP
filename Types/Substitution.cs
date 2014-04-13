namespace NOP
{
	using System;
	using Collections;
	using System.Text;

	/// <summary>
	/// Substitution table provides a mapping from type variables to concrete monotypes.
	/// The substitution table is both the working data structure and the result of
	/// the infrence process.
	/// </summary>
	public class Substitution
	{
		private readonly Map<string, MonoType> _map;
			
		public Substitution (Map<string, MonoType> map)
		{
			_map = map;
		}
		
		/// <summary>
		/// Empty substitution table.
		/// </summary>
		public static Substitution Empty
		{
			get { return new Substitution (Map<string, MonoType>.Empty); }
		}
			
		/// <summary>
		/// Extend the substitution table with specified variable.
		/// </summary>
		public Substitution Extend (string name, MonoType type)
		{
			return new Substitution (_map.Add (name, type));
		}
			
		/// <summary>
		/// Lookup the type variable with the specified name. If the variable with
		/// the given name is not currently in the substitution table a new 
		/// variable is returned.
		/// </summary>
		public MonoType Lookup (string name)
		{
			return _map.Contains (name) ? _map [name] : new MonoType.Var (name);
		}

		public override string ToString()
		{
			return _map.ReduceLeft(new StringBuilder("{ "), 
				(sb, t) => sb.AppendFormat("{0}{1} => {2}", 
					sb.Length > 2 ? ", " : "", t.Item1, t.Item2)) 
				.Append(" }").ToString();
		}
	}		
}