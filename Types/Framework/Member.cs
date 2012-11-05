namespace NOP.Framework
{
	using System.Reflection;
	
	/// <summary>
	/// Member of a class.
	/// </summary>
	public abstract class Member : Definition
	{
		protected Member (MemberInfo mi) : base (mi)
		{
		}
		
		public MemberInfo Info
		{
			get { return _memberInfo; }
		}
	}
} 