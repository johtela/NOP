namespace NOP
{
	using System;
	
	public abstract class SExpr
	{
		public class Literal : SExpr
		{
			public readonly object Value;
		
			public Literal (object value)
			{
				Value = value;
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as Literal;
				return other != null && Value.Equals (other.Value);
			}
			
			public override int GetHashCode ()
			{
				return Value.GetHashCode ();
			}
			
			public override string ToString ()
			{
				return Value.ToString ();
			}
		}

		public class Symbol : SExpr
		{
			public readonly string Name;
		
			public Symbol (string name)
			{
				Name = name;
			}
		
			public override bool Equals (object obj)
			{
				var other = obj as Symbol;
				return other != null && other.Name == Name;
			}
		
			public override int GetHashCode ()
			{
				return Name.GetHashCode ();
			}
			
			public override string ToString ()
			{
				return Name;
			}
		}
	
		public class List : SExpr
		{
			public readonly Collections.List<SExpr> Items;
		
			public List (Collections.List<SExpr> items)
			{
				Items = items;
			}
			
			public override bool Equals (object obj)
			{
				var other = obj as List;
				return other != null && Items.Equals (other.Items);
			}
			
			public override int GetHashCode ()
			{
				return Items.GetHashCode ();
			}
			
			public override string ToString ()
			{
				return Items.ToString ();
			}
		}
	}
}