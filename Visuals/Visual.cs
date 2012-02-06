namespace NOP
{
	using System;
	using System.Linq;

    /// <summary>
    /// Visuals that represent the elements in the program.
    /// </summary>
    public abstract class Visual
    {
        /// <summary>
        /// An abstract method that calculates the size of a visual once it is constructed.
        /// </summary>
        /// <param name="availableSpace">The maximum size available to the visual.
        /// The actual size of the visual should never exceed the available space.
        /// Normally, the size should not fill the whole available size either, because
        /// then nothing else can be rendered into the same space.</param>
        /// <returns>The desired size of the visual that should alwasys be smaller than 
        /// the available space.</returns>
        protected abstract VisualBox CalculateSize(VisualBox availableSpace);

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
            public _Label(string text)
            {
                Text = text;
            }

            /// <summary>
            /// Calculates the size of the label.
            /// </summary>
            /// <param name="availableSize">The space available for the label.</param>
            /// <returns>The size of the rendered label truncated to the available space.
            /// </returns>
            protected override VisualBox CalculateSize(VisualBox availableSize)
            {
                return VisualBox.Empty; //TextRenderer.MeasureText(Text, Settings.Font).Intersect(availableSize);
            }
        }

        /// <summary>
        /// Create a new label.
        /// </summary>
        public Visual Label(string text)
        {
            return new _Label(text);
        }
    }
}
