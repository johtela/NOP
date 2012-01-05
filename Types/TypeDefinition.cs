using System;
using System.Linq;
using System.Reflection;
using NOP.Collections;
using NameDef = System.Tuple<string, NOP.Definition>;

namespace NOP
{
	public abstract class TypeDefinition : Namespace
	{
		private Map<string, Definition> _definitions = Map<string, Definition>.Empty;
		
		protected TypeDefinition (string[] path, Namespace parent, Type type) :
			base(path, parent)
		{
			var bfStatic = BindingFlags.Public | BindingFlags.Static;
			var bfInstance = BindingFlags.Public | BindingFlags.Instance;
			
			var functions = from mi in type.GetMethods (bfStatic)
							select FuncDef(mi);
			
			var values = from pi in type.GetProperties (bfStatic)
						 where !pi.CanWrite
						 select ValDef(pi);
			
			var variables = (from pi in type.GetProperties (bfStatic) 
							where pi.CanWrite
							select VarDef(pi)).Concat (
							from fi in type.GetFields (bfStatic)
							select VarDef(fi));
			
			var constructors = from ci in type.GetConstructors (bfInstance)
							   select ConsDef(ci);
			 
			var methods = from mi in type.GetMethods (bfInstance)
						  select MethDef(mi);
			
			var properties = from pi in type.GetProperties (bfInstance)
							 select PropDef(pi);
			
			_definitions = Map<string, Definition>.FromPairs (functions
				.Concat (values)
				.Concat (variables)
				.Concat (constructors)
				.Concat (methods)
				.Concat (properties));
		}
		
		private NameDef FuncDef (MethodInfo mi)
		{
			return new NameDef (GetSignature (mi), new Function (mi));
		}
		
		private NameDef ValDef (PropertyInfo pi)
		{
			return new NameDef (GetSignature (pi), new Value (pi));
		}
				
		private NameDef VarDef (MemberInfo mi)
		{
			return new NameDef (GetSignature (mi), new Variable (mi));
		}
				
		private NameDef ConsDef (ConstructorInfo ci)
		{
			return new NameDef (GetSignature (ci), new Constructor (ci));
		}

		private NameDef MethDef (MethodInfo mi)
		{
			return new NameDef (GetSignature (mi), new Method (mi));
		}

		private NameDef PropDef (PropertyInfo pi)
		{
			return new NameDef (GetSignature (pi), new Property (pi));
		}

		private string GetSignature (MemberInfo mi)
		{
			var sig = mi.ToString ();
			return sig.Substring (sig.IndexOf (' ') + 1);
		}
	}
}