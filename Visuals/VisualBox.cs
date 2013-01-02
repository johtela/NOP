namespace NOP
{
	using System;
	
	/// <summary>
	/// Structure that is used to layout visuals.
	/// </summary>
	public struct VisualBox
	{
		/// <summary>
		/// The width of the box.
		/// </summary>
		public readonly float Width;
		
		/// <summary>
		/// The height of the box.
		/// </summary>
		public readonly float Height;
		
		/// <summary>
		/// Empty box.
		/// </summary>
		public static readonly VisualBox Empty = new VisualBox (0, 0);
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NOP.VisualBox"/> struct.
		/// </summary>
		/// <param name='width'>The width of the box.</param>
		/// <param name='height'>The height of the box.</param>
		public VisualBox (float width, float height)
		{
			Width = width;
			Height = height;
		}
		
		/// <summary>
		/// Add another box to horizontally to this one.
		/// </summary>
		public VisualBox HAdd (VisualBox other)
		{
			return new VisualBox (Width + other.Width, Height);
		}
		
		/// <summary>
		/// Subtract another box from this one horizontally.
		/// </summary>
		public VisualBox HSub (VisualBox other)
		{
			return new VisualBox (Width - other.Width, Height);
		}
		
		/// <summary>
		/// Returns the horizontal union with another box. This means that the
		/// width of the result is the maximum of the box widths.
		/// </summary>
		public VisualBox HMax (VisualBox other)
		{
			return new VisualBox (Math.Max (Width, other.Width), Height);
		}
		
		/// <summary>
		/// Returns the horizontal intersection with another box. This means that the
		/// width of the result is the minimum of the box widths.
		/// </summary>
		public VisualBox HMin (VisualBox other)
		{
			return new VisualBox (Math.Min (Width, other.Width), Height);
		}

		/// <summary>
		/// Add another box to vertically to this one.
		/// </summary>
		public VisualBox VAdd (VisualBox other)
		{
			return new VisualBox (Width, Height + other.Height);
		}
		
		/// <summary>
		/// Subtract another box from this one vertically.
		/// </summary>
		public VisualBox VSub (VisualBox other)
		{
			return new VisualBox (Width, Height - other.Height);
		}
		
		/// <summary>
		/// Returns the vertical union with another box. This means that the
		/// height of the result is the maximum of the box heights.
		/// </summary>
		public VisualBox VMax (VisualBox other)
		{
			return new VisualBox (Width, Math.Max (Height, other.Height));
		}
		
		/// <summary>
		/// Returns the vertical intersection with another box. This means that the
		/// height of the result is the minimum of the box heights.
		/// </summary>
		public VisualBox VMin (VisualBox other)
		{
			return new VisualBox (Width, Math.Min (Height, other.Height));
		}
		
		/// <summary>
		/// Is this an empty box. A box is empty, if either its width or height
		/// is less or equal to zero.
		/// </summary>
		public bool IsEmpty
		{
			get { return Width <= 0 || Height <= 0; }
		}
		
		public override string ToString ()
		{
			return string.Format ("({0}, {1})", Width, Height);
		}
	}
}