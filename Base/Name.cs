namespace NOP
{
	using System;
	using NOP.Collections;

	/// <summary>
	/// Namespace is the context in which a name is defined. This allows having the same
	/// name in multiple contexts. All namespaces are stored in a global map, and they
	/// are just referred by the names.
	/// </summary>
	public class Namespace : IComparable<Namespace>
	{
		private static Map<string, Namespace> _namespaces = Map<string, Namespace>.Empty;
		public readonly string Value;

		private Namespace (string value)
		{
			Value = value;
		}

		public static Namespace Get (string value)
		{
			var result = _namespaces.TryGetValue (value);

			if (result.HasValue)
				return result;
			var ns = new Namespace (value);
			_namespaces = _namespaces.Add (value, ns);
			return ns;
		}

		public static implicit operator string (Namespace ns)
		{
			return ns.Value;
		}

		public override string ToString ()
		{
			return Value;
		}

		public override bool Equals (object obj)
		{
			var other = obj as Namespace;
			return other != null && other.Value == Value;
		}

		public override int GetHashCode ()
		{
			return Value.GetHashCode ();
		}

		public int CompareTo (Namespace other)
		{
			return Value.CompareTo (other.Value);
		}
	}

	/// <summary>
	/// A name is used to represent symbols in the language. All names have an
	/// associated namespace, and a same name can occur in multiple namespaces
	/// representing different language objects.
	/// </summary>
	public class Name : IComparable<Name>
	{
		public readonly string Value;
		public readonly Namespace Namespace;

		public Name (string value, Namespace ns)
		{
			Value = value;
			Namespace = ns;
		}

		public Name (string value, string ns) : this (value, Namespace.Get (ns)) {}

		public Name (string value) : this (value, null) {}

		public static implicit operator string (Name name)
		{
			return name.Namespace != null ?
				string.Format ("{0}.{1}", name.Namespace, name.Value) :
				name.Value;
		}

		public override bool Equals (object obj)
		{
			var other = obj as Name;
			return other != null && other.Value == Value && other.Namespace == Namespace;
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}

		public override string ToString ()
		{
			return this;
		}

		public int CompareTo (Name other)
		{
			return ToString ().CompareTo (other.ToString ());
		}
	}
}
