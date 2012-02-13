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
		public static readonly int Answer = 42;
		
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
		
		public int Value
		{
			get { return _value; }
			set { _value = value; }
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
		
		private static Class GetFooClass ()
		{
			return  (Class)Namespace.Find ("NOP.Testbench.Foo");
		}
		
		private static Tuple<Class, object, List<object>> GetFoo (int val)
		{
			var fooClass = GetFooClass ();
			var args = List.Create<object> (val);
			
			return Tuple.Create (fooClass, fooClass.GetConstructor (".ctor(Int32)").Create (args), args);
		}
		
		[Test]
		public void TestCallingFunction ()
		{
			Check.AreEqual (42, GetFooClass ().GetFunction ("Bar()").Call (Wrappers.NoArgs));
		}
		
		[Test]
		public void TestReadingValue ()
		{
			Check.AreEqual (42, GetFooClass ().GetValue ("Answer").Get ());
		}
		
		[Test]
		public void TestCallingMethod ()
		{
			var foo = GetFoo (13);
			
			Check.AreEqual (26, foo.Item1.GetMethod ("Add(Int32)").Call (foo.Item2, foo.Item3));
		}
		
		[Test]
		public void TestUsingProperty ()
		{
			var foo = GetFoo (13);
			var prop = foo.Item1.GetProperty ("Value");
			
			Check.AreEqual (13, prop.Get (foo.Item2));
			prop.Set (foo.Item2, 15);
			Check.AreEqual (15, prop.Get (foo.Item2));
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