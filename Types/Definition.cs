using System.Reflection;

namespace NOP
{
	/// <summary>
	/// Modules consist of definitions. A definition is either a value, 
	/// variable or a function.
	/// </summary>
	public abstract class Definition
	{
		protected readonly MemberInfo _memberInfo;
		
		protected Definition (MemberInfo mi)
		{
			_memberInfo = mi;
		}
		
		public override string ToString ()
		{
			var sig = _memberInfo.ToString ();
			return sig.Substring (sig.IndexOf (' ') + 1);
		}
		
		public static D Get<T, D> (string name) where D : Definition
		{
			return (D)TypeDefinition._typeInfos [typeof(T)].Definitions [name];
		}
	}
}