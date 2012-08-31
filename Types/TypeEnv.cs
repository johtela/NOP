namespace NOP
{
	using System;
	using System.Linq;
	using Collections;
	using StrSeq=System.Collections.Generic.IEnumerable<string>;
	
	/// <summary>
	/// The type environment maps expression variables (NOT type variables) to polytupes.
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
		/// Add a variable with the given type to the enironment.
		/// </summary>
		public TypeEnv Add (string name, Polytype type)
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
			private static StrSeq GetTypeVars (MonoType[] parameters)
			{
				foreach (var par in parameters)
					if (par is MonoType.Var) yield return (par as MonoType.Var).Name;
			}

			private static Polytype Pt (MonoType result, params MonoType[] parameters)
			{
				return new Polytype (Lambda (List.Create (parameters), result), GetTypeVars (parameters));
			}

			private static Tuple<string, Polytype> Ft(string funcName, Polytype pt)
			{
				return Tuple.Create (funcName, pt);
			}

			public static readonly TypeEnv Value =
				new TypeEnv (Map<string, Polytype>.FromPairs (
					Ft ("set!", Pt (Constant ("System.Void"), Variable ("a"), Variable ("a"))),
					Ft ("eq?", Pt (Constant ("System.Boolean"), Variable ("a"), Variable ("a")))
				));
		}
	}
}