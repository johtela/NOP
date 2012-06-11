namespace NOP
{
	using System;

	public class UnificationError : Exception
	{
		public readonly MonoType Type1;
		public readonly MonoType Type2;
			
		public UnificationError (MonoType type1, MonoType type2) 
				: base (string.Format ("Could not unify type {0} with {1}", type1, type2))
		{
			Type1 = type1;
			Type2 = type2;
		}
	}		
}

