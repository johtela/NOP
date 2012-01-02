using System;
using System.Collections.Generic;
using System.Linq;
using NOP.Collections;

namespace NOP
{
	public class Namespace
	{
		private string[] _path;
		private Map<string, Namespace> _namespaces = Map<string, Namespace>.Empty;
		private static Namespace _root = new Namespace ();
		
		private Namespace (string[] path, Namespace parent)
		{
			_path = path;
			parent._namespaces = parent._namespaces.Add (path [path.Length - 1], this);
		}
		
		private Namespace ()
		{
			_path = new string[0];
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
			{
				var ns = GetOrCreate (type.Namespace);
			}
			return _root;
		}
	}
}