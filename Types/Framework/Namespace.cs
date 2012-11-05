using System;
using System.Collections.Generic;
using System.Linq;
using NOP.Collections;

namespace NOP.Framework
{
	public class Namespace
	{
		private string[] _path;
		private static Namespace _root = new Namespace ();
		protected Map<string, Namespace> _namespaces = Map<string, Namespace>.Empty;
		
		protected Namespace (string[] path, Namespace parent)
		{
			_path = path;
			parent._namespaces = parent._namespaces.Add (path [path.Length - 1], this);
		}
		
		private Namespace ()
		{
			_path = new string[0];
		}
		
		public override bool Equals (object obj)
		{
			var other = obj as Namespace;
			return other != null && ToString () == other.ToString ();
		}
		
		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}
		
		public override string ToString ()
		{
			return string.Join (".", _path);
		}
		
		private static Namespace GetOrCreate (string path)
		{
			var ns = _root;
			var names = path.Split ('.');
			
			for (int i = 0; i < names.Length; i++)
			{
				var name = names [i];
				ns = ns._namespaces.Contains (name) ?
					ns._namespaces [name] :
					new Namespace (names.Segment (0, i + 1), ns);
			}
			return ns;
		}
		
		public static Namespace FromTypes (IEnumerable<Type> types)
		{
			foreach (var type in types)
				if (type.Namespace != null && !type.IsNested)
				{
					var ns = GetOrCreate (type.Namespace);
					TypeDefinition.CreateType (ns, type);
				}
			return _root;
		}

		public static Namespace Find (string path)
		{
			var ns = _root;
			try
			{
				foreach (var name in path.Split ('.'))
					ns = ns._namespaces [name];
				return ns;
			} catch (KeyNotFoundException)
			{
				return null;
			}
		}
		
		public string[] Path
		{
			get { return _path; }
		}
		
		public Map<string, Namespace> Namespaces
		{
			get { return _namespaces; }
		}
		
		public static Namespace Root
		{
			get { return _root; }
		}
	}
}
