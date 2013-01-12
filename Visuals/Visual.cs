namespace NOP
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NOP.Collections;
    using System.Drawing;
    using System.Drawing.Drawing2D;
	
	/// <summary>
	/// Enumeration for defining the stack direction.
	/// </summary>
	public enum StackDirection { Horizontal, Vertical };
	
	/// <summary>
	/// Horizontal alignment of the items in a stack.
	/// </summary>
	public enum HAlign { Left, Center, Right };
	
	/// <summary>
	/// Vertical alignment of the items in a stack.
	/// </summary>
	public enum VAlign { Top, Center, Bottom };
		
	/// <summary>
	/// Visuals that represent the elements in the program.
	/// </summary>
	public abstract class Visual
	{
        private static Font _defaultFont = new Font ("Consolas", 11);
        private static Brush _defaultBrush = Brushes.Black;
        private static Pen _rectPen = new Pen (Color.Gray, 1) { DashStyle = DashStyle.Dot };

		/// <summary>
		/// An abstract method that calculates the size of a visual once it is constructed.
		/// </summary>
		/// <param name="context">The Cairo context to which the visual is eventually
		/// drawn to.</param>
		/// <returns>The desired size of the visual that should alwasys be smaller than 
		/// the available space.</returns>
		protected abstract VBox CalculateSize (Graphics gr);
		
		/// <summary>
		/// Draw the visual into specified context using the available size.
		/// </summary>
		/// <param name="context">The Cairo context to which the visual is drawn to.</param>
		/// <param name='availableSize'>The available size into which the visual should
		/// fit.</param>
		protected abstract void Draw (Graphics gr, VBox availableSize);
		
        public void Render (Graphics gr, VBox availableSize)
        {
            Draw (gr, availableSize);
        }

        public VBox GetSize (Graphics gr)
        {
            return CalculateSize (gr);
        }

		/// <summary>
		/// A label that renders some static text into the output.
		/// </summary>
		private sealed class _Label : Visual
		{
			/// <summary>
			/// The text to be rendered.
			/// </summary>
			public readonly string Text;

			/// <summary>
			/// A constructor for the label.
			/// </summary>
			public _Label (string text)
			{
				Text = text;
			}

			/// <summary>
			/// Calculates the size of the label.
			/// </summary>
			protected override VBox CalculateSize (Graphics gr)
			{
				return new VBox (gr.MeasureString(Text, _defaultFont).Width , _defaultFont.Height);
			}
			
			/// <summary>
			/// Draw the label into the specified context.
			/// </summary>
            protected override void Draw (Graphics gr, VBox availableSize)
            {
                var pos = new PointF (0, 0);
                gr.DrawString (Text, _defaultFont, _defaultBrush, pos);
                //gr.DrawRectangle (_rectPen, 0, 0, availableSize.Width, availableSize.Height);
            }
		}
		
		/// <summary>
		/// Stack of visuals that are laid out either horizontally (left to right) or
		/// vertically (top to bottom).
		/// </summary>
		private sealed class _Stack : Visual
		{
			/// <summary>
			/// The visuals in the stack.
			/// </summary>
			public readonly NOPList<Visual> Items;
			
			/// <summary>
			/// The direction of the stack (horizontal or vertical)
			/// </summary>
			public readonly StackDirection Direction;
			
			/// <summary>
			/// This setting controls how the items in stack are aligned horizontally,
			/// that is, whether they are aligned by their left or right edges or centered. 
			/// The setting only has effect, if the stack is vertical.
			/// </summary>
			public readonly HAlign HorizAlign;
			
			/// <summary>
			/// This setting controls how the items in stack are aligned vertically,
			/// that is, whether they are aligned by their top or bottom edges or centered. 
			/// The setting only has effect, if the stack is horizontal.
			/// </summary>
			public readonly VAlign VertAlign;
			
			/// <summary>
			/// Initializes a new stack.
			/// </summary>
			public _Stack (NOPList<Visual> items, StackDirection direction, HAlign horizAlign,
				VAlign vertAlign)
			{
				Items = items;
				Direction = direction;
				HorizAlign = horizAlign;
				VertAlign = vertAlign;
			}
			
			/// <summary>
			/// Override to calculates the size of the visual. 
			/// </summary>
			/// <description>
			/// If the stack is horizontal, the width of the stack is the sum of the 
			/// widths of the visuals in it. The height of the stack is the height of 
			/// the tallest item.<para/>
			/// If the stack is vertical, the height of the stack is the sum of the 
			/// heights of the visuals in it. The width of the stack is the with of 
			/// the widest item.
			/// </description>
            protected override VBox CalculateSize (Graphics gr)
			{
				return Items.Fold (VBox.Empty, (acc, v) => 
				{
					var box = v.CalculateSize (gr);
					return Direction == StackDirection.Horizontal ?
						acc.VMax (box).HAdd (box) :
						acc.HMax (box).VAdd (box);
				});
			}
			
			/// <summary>
			/// Calulate the horizontal offset of a visual based on the alignment.
			/// </summary>
			private float DeltaX (float outerWidth, float innerWidth)
			{
				switch (HorizAlign)
				{
					case HAlign.Center: return (outerWidth - innerWidth) / 2;
					case HAlign.Right: return outerWidth - innerWidth;
					default: return 0;
				}
			}
			
			/// <summary>
			/// Calulate the vertical offset of a visual based on the alignment.
			/// </summary>
			private float DeltaY (float outerHeight, float innerHeight)
			{
				switch (VertAlign)
				{
					case VAlign.Center: return (outerHeight - innerHeight) / 2;
					case VAlign.Bottom: return outerHeight - innerHeight;
					default: return 0;
				}
			}
			
			/// <summary>
			/// Draw the stack into the specified context.
			/// </summary>
            protected override void Draw (Graphics gr, VBox availableSize)
			{
				var stack = CalculateSize (gr);
				
				foreach (Visual visual in Items)
				{
					if (availableSize.IsEmpty) break;
					
					var inner = visual.CalculateSize (gr);
					var outer = Direction == StackDirection.Horizontal ?
						new VBox (inner.Width, stack.Height) :
						new VBox (stack.Width, inner.Height);
					var st = gr.Save ();
					gr.TranslateTransform (DeltaX (outer.Width, inner.Width), 
						DeltaY (outer.Height, inner.Height));
					visual.Render (gr, inner);
					gr.Restore (st);
					
					if (Direction == StackDirection.Horizontal)
					{
						gr.TranslateTransform (outer.Width, 0);
						availableSize = availableSize.HSub (outer);
					}
					else
					{
						gr.TranslateTransform (0, outer.Height);
						availableSize = availableSize.VSub (outer);
					}
				}
			}
		}

        /// <summary>
        /// Use the visual inside a S-expression.
        /// </summary>
        class _Indirect : Visual
        {
            public readonly SExpr SExpr;

            public _Indirect (SExpr sexp)
            {
                SExpr = sexp;
            }

            protected override VBox CalculateSize (Graphics gr)
            {
                return SExpr.Depiction.CalculateSize (gr);
            }

            protected override void Draw (Graphics gr, VBox availableSize)
            {
                SExpr.Depiction.Draw (gr, availableSize);
            }
        }

		/// <summary>
		/// Create a new label.
		/// </summary>
		public static Visual Label (string text)
		{
			return new _Label (text);
		}
		
		/// <summary>
		/// Create a horizontal stack.
		/// </summary>
		public static Visual HorizontalStack (VAlign alignment, IEnumerable<Visual> visuals)
		{
			return new _Stack (List.Create (visuals), StackDirection.Horizontal, HAlign.Left, alignment);
		}
		
		/// <summary>
		/// Create a horizontal stack.
		/// </summary>
		public static Visual HorizontalStack (VAlign alignment, params Visual[] visuals)
		{
			return new _Stack (List.Create (visuals), StackDirection.Horizontal, HAlign.Left, alignment);
		}
		
		/// <summary>
		/// Create a vertical stack.
		/// </summary>
		public static Visual VerticalStack (HAlign alignment, IEnumerable<Visual> visuals)
		{
			return new _Stack (List.Create (visuals), StackDirection.Vertical, alignment, VAlign.Top);
		}
		
		/// <summary>
		/// Create a vertical stack.
		/// </summary>
		public static Visual VerticalStack (HAlign alignment, params Visual[] visuals)
		{
			return new _Stack (List.Create (visuals), StackDirection.Vertical, alignment, VAlign.Top);
		}

        /// <summary>
        /// Create an indirect visual.
        /// </summary>
        public static Visual Indirect(SExpr sexp)
        {
            return new _Indirect (sexp);
        }

        /// <summary>
        /// Create a visual for a symbol S-expression.
        /// </summary>
        public static Visual Symbol (SExpr sexp)
        {
            return Label (((SExpr.Symbol)sexp).Name);
        }

        /// <summary>
        /// Create a visual for a literal S-expression.
        /// </summary>
        public static Visual Literal (SExpr sexp)
        {
            return Label (((SExpr.Literal)sexp).Value.ToString ());
        }

        /// <summary>
        /// Create a horizontal list of S-expressions.
        /// </summary>
        public static Visual HList (SExpr sexp)
        {
            return HorizontalStack (VAlign.Bottom, FromSExpList ((SExpr.List)sexp));
        }

        /// <summary>
        /// Return a vertical list of S-expressions.
        /// </summary>
        public static Visual VList (SExpr sexp)
        {
            return VerticalStack (HAlign.Left, FromSExpList ((SExpr.List)sexp));
        }

        /// <summary>
        /// Surrond a visual horizontally by parentheses.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Visual Parenthesize (Visual v)
        {
            return HorizontalStack (VAlign.Center, Label ("("), v, Label (")"));
        }

        /// <summary>
        /// Map a list of S-expressions to a sequence of visuals.
        /// </summary>
        private static IEnumerable<Visual> FromSExpList (SExpr.List list)
        {
            return list.Items.Map (se => Indirect (se));
        }
	}
}