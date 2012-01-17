namespace NOP.Testbench
{
	using System;
	using System.Linq;
	using System.Reflection;
	using NOP;
	using NOP.Collections;

	public class Foo
	{
		private int _value;
		
		public Foo (int val)
		{
			_value = val;
		}
		
		public static int Bar ()
		{
			return 42;
		}
		
		public int Add (int other)
		{
			return _value + other;
		}
	}

	public class TypeDefinitionTests
	{
		private int tabs;
		
		public TypeDefinitionTests ()
		{
			var assy = Assembly.GetExecutingAssembly ();
			Namespace.FromTypes (assy.GetTypes ());
		}
		
		[Test]
		public void OutputAllNamespaces ()
		{
			tabs = 0;
			OutputNamespace (Namespace.Root);
		}
		
		[Test]
		public void TestCallingFunction ()
		{
			var fooClass = (Class)Namespace.Find ("NOP.Testbench.Foo");
			
			Check.AreEqual (42, fooClass.GetFunction ("Bar()").Call (Expression.NoArgs));
		}

		[Test]
		public void TestCallingMethod ()
		{
			var fooClass = (Class)Namespace.Find ("NOP.Testbench.Foo");
			var args = List.Create<object> (13);
			
			var foo = fooClass.GetConstructor (".ctor(Int32)").Create (args);
			Check.AreEqual (26, fooClass.GetMethod ("Add(Int32)").Call (foo, args));
		}
				
		#region Output functions
		
		private void Indent ()
		{
			tabs += 2;
		}
	
		private void UnIndent ()
		{
			tabs -= 2;
		}
	
		private void Output (string txt)
		{
			Console.WriteLine (txt.PadLeft (txt.Length + tabs));
		}
	
		private void OutputNamespace (Namespace ns)
		{
			Indent ();
			Output (ns.Path.DefaultIfEmpty ("<root>").Aggregate ((res, name) => res + "." + name));
		
			var td = ns as TypeDefinition;
		
			if (td != null)
			{
				Indent ();
				foreach (var def in td.Definitions)
					Output (def.Item1);
				UnIndent ();
			}
			foreach (var subns in ns.Namespaces)
				OutputNamespace (subns.Item2);
			UnIndent ();
		}
		
		#endregion
	}
}