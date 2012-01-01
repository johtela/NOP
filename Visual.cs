using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace NOP
{
    /// <summary>
    /// Visuals that represent the elements in the program.
    /// </summary>
    /// <remarks>The visual is an immutable case object that should be able to calculate 
    /// its size and draw itself to a given graphics canvas. The size is initialized after
    /// the object is constructed, so it cannot be a readonly field. However, it is immutable
    /// in the sense that it never changes once it is calculated.</remarks>
    public abstract class Visual
    {
        /// <summary>
        /// The settings that specify how the visual should be drawn. For example,
        /// what Pen, Brush, or Font to use.
        /// </summary>
        public readonly VisualSettings Settings;
        /// <summary>
        /// The size of the visual that is initialized after all the constructors
        /// have been called. The size is a private field, but it stays immutable 
        /// nonetheless.
        /// </summary>
        private Size _size;

        /// <summary>
        /// Private constructor for the Visual base class.
        /// </summary>
        /// <param name="settings">The settings to be used to render the visual. 
        /// Should not be null.</param>
        private Visual(VisualSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            Settings = settings;
        }

        /// <summary>
        /// An abstract method that calculates the size of a visual once it is
        /// constructed. This method is called by the <see cref="WithSize"/> method
        /// that is the last step of a visual construction.
        /// </summary>
        /// <param name="availableSpace">The maximum size available to the visual.
        /// The actual size of the visual should never exceed the available space.
        /// Normally, the size should not fill the whole available size either, because
        /// then nothing else can be rendered into the same space.</param>
        /// <returns>The desired size of the visual that should alwasys be smaller than 
        /// the available space.</returns>
        protected abstract Size CalculateSize(Size availableSpace);

        /// <summary>
        /// A label that renders some static text into the output. The text cannot be
        /// changed afterwards.
        /// </summary>
        private sealed class _Label : Visual
        {
            /// <summary>
            /// The text to be rendered.
            /// </summary>
            public readonly string Text;

            /// <summary>
            /// A constructor that initializes all the fields.
            /// </summary>
            /// <param name="settings">The settings used to render the label</param>
            /// <param name="text">The text to be rendered.</param>
            public _Label(VisualSettings settings, string text) : base(settings)
            {
                Text = text;
            }

            /// <summary>
            /// Calculates the size of the label.
            /// </summary>
            /// <param name="availableSize">The space available for the label.</param>
            /// <returns>The size of the rendered label truncated to the available space.
            /// </returns>
            protected override Size CalculateSize(Size availableSize)
            {
                return TextRenderer.MeasureText(Text, Settings.Font).Intersect(availableSize);
            }
        }

        /// <summary>
        /// A helper method that initializes the size of a visual by calling the
        /// <see cref="CalculateSize"/> method.
        /// </summary>
        /// <param name="visual">The visual to be initialized.</param>
        /// <param name="availableSize">The size available to the visual.</param>
        /// <returns>The same visual that was passed in the <paramref name="visual"/>
        /// parametr.</returns>
        private Visual WithSize(Visual visual, Size availableSize)
        {
            visual._size = visual.CalculateSize(availableSize);
            return visual;
        }

        /// <summary>
        /// Create a new label.
        /// </summary>
        /// <param name="settings">The settings to be used to render the label.</param>
        /// <param name="text">The text of the label.</param>
        /// <param name="availableSize">The size available for the label.</param>
        /// <returns>A new label that is initialized according to the paramters given.
        /// </returns>
        public Visual Label(VisualSettings settings, string text, Size availableSize)
        {
            return WithSize(new _Label(settings, text), availableSize);
        }
    }
}
