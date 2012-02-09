namespace NOP
{
	using System;
	using System.Linq;
	using Cairo;
	using NOP.Collections;
	
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
		/// <summary>
		/// An abstract method that calculates the size of a visual once it is constructed.
		/// </summary>
		/// <param name="context">The Cairo context to which the visual is eventually
		/// drawn to.</param>
		/// <returns>The desired size of the visual that should alwasys be smaller than 
		/// the available space.</returns>
		protected abstract VisualBox CalculateSize (Context context);
		
		/// <summary>
		/// Draw the visual into specified context using the available size.
		/// </summary>
		/// <param name="context">The Cairo context to which the visual is drawn to.</param>
		/// <param name='availableSize'>The available size into which the visual should
		/// fit.</param>
		protected abstract void Draw (Context context, VisualBox availableSize);
		
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
			protected override VisualBox CalculateSize (Context context)
			{
				return new VisualBox (context.TextExtents (Text).Width, context.FontExtents.Height);
			}
			
			/// <summary>
			/// Draw the label into the specified context.
			/// </summary>
			protected override void Draw (Context context, VisualBox availableSize)
			{
				context.MoveTo (0, availableSize.Height);
				context.ShowText (Text);
			}
		}
		
		private class _Stack : Visual
		{
			public readonly List<Visual> Items;
			public readonly StackDirection Direction;
			public readonly HAlign HorizAlign;
			public readonly VAlign VertAlign;
			
			public _Stack (List<Visual> items, StackDirection direction, HAlign horizAlign,
				VAlign vertAlign)
			{
				Items = items;
				Direction = direction;
				HorizAlign = horizAlign;
				VertAlign = vertAlign;
			}
			
			protected override VisualBox CalculateSize (Context context)
			{
				return Items.Fold (VisualBox.Empty, (acc, v) => 
				{
					var box = v.CalculateSize (context);
					return Direction == StackDirection.Horizontal ?
						acc.VMax (box).HAdd (box) :
						acc.HMax (box).VAdd (box);
				});
			}
			
			private double DeltaX (double outerWidth, double innerWidth)
			{
				switch (HorizAlign)
				{
					case HAlign.Center: return (outerWidth - innerWidth) / 2;
					case HAlign.Right: return outerWidth - innerWidth;
					default: return 0;
				}
			}
			
			private double DeltaY (double outerHeight, double innerHeight)
			{
				switch (VertAlign)
				{
					case VAlign.Center: return (outerHeight - innerHeight) / 2;
					case VAlign.Bottom: return outerHeight - innerHeight;
					default: return 0;
				}
			}
			
			protected override void Draw (Context context, VisualBox availableSize)
			{
				var stack = CalculateSize (context);
				
				foreach (Visual visual in Items)
				{
					if (availableSize.IsEmpty) break;
					
					var inner = visual.CalculateSize (context);
					var outer = Direction == StackDirection.Horizontal ?
						new VisualBox (inner.Width, stack.Height) :
						new VisualBox (stack.Width, inner.Height);
					context.Save ();
					context.Translate (DeltaX (outer.Width, inner.Width), 
						DeltaY (outer.Height, inner.Height));
					visual.Draw (context, inner);
					context.Restore ();
					
					if (Direction == StackDirection.Horizontal)
					{
						context.Translate (outer.Width, 0);
						availableSize = availableSize.HSub (outer);
					}
					else
					{
						context.Translate (0, outer.Height);
						availableSize = availableSize.VSub (outer);
					}
				}
			}
		}

		/// <summary>
		/// Create a new label.
		/// </summary>
		public Visual Label (string text)
		{
			return new _Label (text);
		}
	}
}