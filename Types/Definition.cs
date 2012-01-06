using System.Reflection;

namespace NOP
{
	/// <summary>
	/// Modules consist of definitions. A definition is either a value, 
	/// variable or a function.
	/// </summary>
	public abstract class Definition
	{
		/// <summary>
		/// Gets the signature of a member without the return type.
		/// </summary>
		/// <returns>
		/// The signature of the member in a standard format. 
		/// For example: Equals(System.Object)
		/// </returns>
		/// <param name='mi'>The member which signature is returned.</param>
		public static string GetSignature (MemberInfo mi)
		{
			var sig = mi.ToString ();
			return sig.Substring (sig.IndexOf (' ') + 1);
		}
	}
}