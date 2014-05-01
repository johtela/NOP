namespace NOP
{
	using System;
	using Collections;
	using IO;
	using Parsing;
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
				return string.Format("{{{0}:{1}}}", Value.GetType().FullName, 
					ConvertLiteral.ToString (Value));
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
				return other != null && other.Name.Equals (Name);
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
			public readonly Sequence<SExpr> Items;

			public List (Sequence<SExpr> items)
			{
				Items = items;
				Depiction = Visual.Parenthesize (Visual.HList (Items, ","));
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
				return Items.ToString ("(", ")", " ");
			}
		}

		public static SExpr Lit (object value)
		{
			return new Literal (value);
		}

		public static SExpr Sym (string name)
		{
			return new Symbol (name);
		}

		public static SExpr Lst (params SExpr[] items)
		{
			return new List (Sequence.Create (items));
		}

		public static SExpr Lst (ISequence<SExpr> items)
		{
			return new List (Sequence.Create (items));
		}
	}
}