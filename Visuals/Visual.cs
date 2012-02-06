namespace NOP
{
	using System;
	using System.Linq;
	using Cairo;
	
	/// <summary>
	/// Enumeration for defining the stack direction.
	/// </summary>
	public enum StackDirection { Horizontal, Vertical };
	
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
		/// <returns>The actual size taken by the visual after it is drawn.</returns>
		protected abstract VisualBox Draw (Context context, VisualBox availableSize);
		
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
			protected override VisualBox Draw (Context context, VisualBox availableSize)
			{
				var size = CalculateSize (context).VMax (availableSize);
				context.MoveTo (0, size.Height);
				context.ShowText (Text);
				return size;
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