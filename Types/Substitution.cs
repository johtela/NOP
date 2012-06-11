namespace NOP
{
	using System;
	using Collections;

	/// <summary>
	/// Substitution of type variables is a map from strings to ExprType.
	/// </summary>
	public class Substitution
	{
		private readonly Map<string, MonoType> _map;
			
		private Substitution (Map<string, MonoType> map)
		{
			_map = map;
		}
		
		public static Substitution Empty
		{
			get { return new Substitution (Map<string, MonoType>.Empty); }
		}
			
		public Substitution Extend (string name, MonoType type)
		{
			return new Substitution (_map.Add (name, type));
		}
			
		public MonoType Lookup (string name)
		{
			return _map.Contains (name) ? _map [name] : new MonoType.Var (name);
		}
	}		
}