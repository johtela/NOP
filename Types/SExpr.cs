namespace NOP
{
	using System;
    using Visuals;
	
	/// <summary>
	/// The definition of the LISP S-expression. The basic element from which 
	/// the programs are constructed. Like text in the traditional programming
	/// languages.
	/// </summary>
	public abstract class SExpr
	{
        public Func<SExpr, Visual> GenerateVisual { get; set; }

		/// <summary>
		/// Literal value.
		/// </summary>
		public class Literal : SExpr
		{
			public readonly object Value;
		
			public Literal (object value)
			{
				Value = value;
                GenerateVisual = DefaultVisualGenerator.Literal;
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

		/// <summary>
		/// Symbol with given name.
		/// </summary>
		public class Symbol : SExpr
		{
			public readonly string Name;
		
			public Symbol (string name)
			{
				Name = name;
                GenerateVisual = DefaultVisualGenerator.Symbol;
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

		/// <summary>
		/// List of other S-expressions.
		/// </summary>
		public class List : SExpr
		{
			public readonly Collections.List<SExpr> Items;
		
			public List (Collections.List<SExpr> items)
			{
				Items = items;
                GenerateVisual = DefaultVisualGenerator.HList;
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