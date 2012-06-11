namespace NOP
{
	using System;
	using System.Linq;
	using Collections;

	public class TypeEnv
	{
		private readonly Map<string, Polytype> _map;
			
		private TypeEnv (Map<string, Polytype> map)
		{
			_map = map;
		}
		
		public Polytype Find (string name)
		{
			return _map [name];
		}
			
		public bool Contains (string name)
		{
			return _map.Contains (name);
		}
			
		public TypeEnv Add (string name, Polytype type)
		{
			return new TypeEnv (_map.Add (name, type));
		}
			
		public Set<string> GetTypeVars ()
		{
			return _map.Values.Aggregate (Set<string>.Empty, (s, pt) => s + pt.GetTypeVars ());
		}
	}
}