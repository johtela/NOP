namespace NOP.Testbench
{
	using System;
	using System.Linq;
	using System.Reflection;
	using NOP;
	using NOP.Collections;

	public class TypeDefinitionTests
	{
		private int tabs;
		
		public TypeDefinitionTests ()
		{
		}
		
		[Test]
		public void CreateDefinitionsForLoadedAssemblies ()
		{
			var assy = Assembly.GetExecutingAssembly ();
			Namespace.FromTypes (assy.GetTypes ());
			OutputAllNamespaces ();
		}
		
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
		
		private void OutputAllNamespaces ()
		{
			tabs = 0;
			OutputNamespace (Namespace.Root);
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
	}
}