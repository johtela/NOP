using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NOP.Collections;
using NameDef = System.Tuple<string, NOP.Definition>;
using NameType = System.Tuple<string, NOP.TypeDefinition>;

namespace NOP
{
	/// <summary>
	/// Type definition class represents a type that is defined inside a namespace.
	/// </summary>
	public abstract class TypeDefinition : Namespace
	{
		protected readonly Type _type;
		protected const BindingFlags _bfStatic = BindingFlags.Public | BindingFlags.Static;
		protected const BindingFlags _bfInstance = BindingFlags.Public | BindingFlags.Instance;
		protected Map<string, Definition> _definitions = Map<string, Definition>.Empty;
			
		/// <summary>
		/// Create a new type definition based on the reflected type.
		/// </summary>
		/// <param name='path'>The full namespace path.</param>
		/// <param name='parent'>Parent namespace object.</param>
		/// <param name='type'>The type object from which the object is initialized.</param>
		protected TypeDefinition (Namespace parent, Type type) :
			base(GetPath(parent, type), parent)
		{
			_type = type;
		}
		
		private static string[] GetPath (Namespace parent, Type type)
		{
			var plen = parent.Path.Length;
			var path = new string[plen + 1];
			parent.Path.CopyTo (path, 0);
			path [plen] = type.Name;
			return path;
		}
		
		private static NameDef NewDef (Definition def)
		{
			return new NameDef (def.ToString (), def);
		}
		
		protected IEnumerable<NameDef> Functions ()
		{
			return from mi in _type.GetMethods (_bfStatic)
				   select NewDef(new Function (mi));
		}
		
		protected IEnumerable<NameDef> Values ()
		{
			return from pi in _type.GetProperties (_bfStatic)
				   where !pi.CanWrite
				   select NewDef(new Value (pi));
		}
				
		protected IEnumerable<NameDef> Variables ()
		{
			return (from pi in _type.GetProperties (_bfStatic)
				   where pi.CanWrite
				   select NewDef(new Variable (pi))).Concat (
				   from fi in _type.GetFields (_bfStatic)
				   select NewDef(new Variable (fi)));
		}
				
		protected IEnumerable<NameDef> Constructors ()
		{
			return from ci in _type.GetConstructors (_bfInstance)
				   select NewDef(new Constructor (ci));
		}

		protected IEnumerable<NameDef> Methods ()
		{
			return from mi in _type.GetMethods (_bfInstance)
				   select NewDef(new Method (mi));
		}

		protected IEnumerable<NameDef> Properties ()
		{
			return from pi in _type.GetProperties (_bfInstance)
				   select NewDef(new Property (pi));
		}
		
		protected void AddNestedTypes ()
		{
			foreach (var nt in _type.GetNestedTypes())
			{
				_namespaces = _namespaces.Add (nt.Name, CreateType (this, nt));
			}
		}
					
		public static TypeDefinition CreateType (Namespace parent, Type type)
		{
			if (type.IsClass)
				return new Class (parent, type);
			if (type.IsInterface)
				return new Interface (parent, type);
			if (type.IsValueType)
				return new Struct (parent, type);
			if (type.IsEnum)
				return new Enum (parent, type);
			throw new ArgumentException ("Unknown type", "type");
		}
		
		public Map<string, Definition> Definitions
		{
			get { return _definitions; }
		}
	}
}