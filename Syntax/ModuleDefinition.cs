namespace NOP
{
	using System;
	using NOP.Collections;

	public class ModuleDefinition : Definition
	{
		public readonly List<Definition> Members;
		
		public ModuleDefinition (List<SExpr> moduleDef) : base (moduleDef)
		{
			var sexps = moduleDef.Rest;
		}
	}
}

