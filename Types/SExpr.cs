namespace NOP
{
	using System;
	using Collections;
	using Visuals;
	
	/// <summary>
	/// The definition of the LISP S-expression. The basic element from which 
	/// the programs are constructed. Like text in the traditional programming
	/// languages.
	/// </summary>
	public abstract class SExpr
	{
		internal Visual Depiction { get; set; }
		internal Visual Editor { get; set; }

		/// <summary>
		/// Literal value.
		/// </summary>
		public class Literal : SExpr
		{
			public readonly object Value;

			public Literal (object value)
			{
				Value = value;
				Depiction = Visual.Literal (this);
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
				Depiction = Visual.Symbol (this);
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
			public readonly NOPList<SExpr> Items;

			public List (NOPList<SExpr> items)
			{
				Items = items;
				Depiction = Visual.Parenthesize (Visual.HList (Items));
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

		public NOPList<SExpr> AsList
		{
			get { return (this as List).Items; }
		}
	}
}