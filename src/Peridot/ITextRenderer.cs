using System.Drawing;
using System.Numerics;

namespace Peridot
{
    /// <summary>
    /// Represents an interface to a text renderer.
    /// </summary>
    public interface ITextRenderer : IDisposable
    {
        /// <summary>
        /// A bool indicating whether this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">The text color.</param>
        /// <param name="rotation">The rotation of the drawing.</param>
        /// <param name="origin">The center of the rotation. (0, 0) for default.</param>
        /// <param name="scale">The scaling of this drawing.</param>
        /// <param name="layerDepth">The layer depth of this drawing.</param>
        public void DrawString(
            IFont font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth);

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen</param>
        /// <param name="color">The text color.</param>
        public void DrawString(
            IFont font,
            int fontSize,
            string text,
            Vector2 position,
            Color color)
        {
            DrawString(font, fontSize, text, position, color, 0, Vector2.Zero, Vector2.One, 0);
        }

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen</param>
        /// <param name="color">The text color.</param>
        /// <param name="rotation">The rotation of the drawing.</param>
        /// <param name="origin">The center of the rotation. (0, 0) for default.</param>
        /// <param name="scale">The scaling of this drawing.</param>
        /// <param name="layerDepth">The layer depth of this drawing.</param>
        public void DrawString(
            IFont font, 
            int fontSize, 
            string text, 
            Vector2 position, 
            Color color,
            float rotation, 
            Vector2 origin, 
            float scale, 
            float layerDepth)
        {
            DrawString(font, fontSize, text, position, color, rotation, origin, new Vector2(scale), layerDepth);
        }
    }

    /// <summary>
    /// Represents an interface to a text renderer.
    /// </summary>
    public interface ITextRenderer<TFont> : ITextRenderer where TFont : notnull, IFont
    {
        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">The text color.</param>
        /// <param name="rotation">The rotation of the drawing.</param>
        /// <param name="origin">The center of the rotation. (0, 0) for default.</param>
        /// <param name="scale">The scaling of this drawing.</param>
        /// <param name="layerDepth">The layer depth of this drawing.</param>
        public abstract void DrawString(
            TFont font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth);

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen</param>
        /// <param name="color">The text color.</param>
        public void DrawString(TFont font, int fontSize, string text, Vector2 position, Color color)
        {
            DrawString(font, fontSize, text, position, color, 0, new Vector2(), new Vector2(1), 0);
        }

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen</param>
        /// <param name="color">The text color.</param>
        /// <param name="rotation">The rotation of the drawing.</param>
        /// <param name="origin">The center of the rotation. (0, 0) for default.</param>
        /// <param name="scale">The scaling of this drawing.</param>
        /// <param name="layerDepth">The layer depth of this drawing.</param>
        public void DrawString(TFont font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
        {
            DrawString(font, fontSize, text, position, color, rotation, origin, new Vector2(scale), layerDepth);
        }
    }
}
