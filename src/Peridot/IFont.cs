using System.Numerics;

namespace Peridot
{
    /// <summary>
    /// Represents a font to use in <see cref="SpriteBatch{TTexture}"/>.
    /// </summary>
    public interface IFont : IDisposable
    {
        /// <summary>
        /// Gets the size of a string when rendered in this font.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="fontSize">The font size to measure.</param>
        /// <returns>The size of <paramref name="text"/> rendered with a <see cref="ITextRenderer"/> 
        /// with <paramref name="fontSize"/> font size.</returns>
        public Vector2 MeasureString(string text, int fontSize);
    }
}
