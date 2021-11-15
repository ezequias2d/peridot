using System.Drawing;
using System.Numerics;
using Veldrid;

namespace Peridot.Veldrid
{
    /// <inheritdoc/>
    public class TextRenderer : ITextRenderer<Font>
    {
        private readonly VeldridSpriteBatch _sb;
        private readonly FontStashRenderer _textRenderer;

        /// <summary>
        /// Creates a new instance of <see cref="TextRenderer"/>.
        /// </summary>
        /// <param name="gd"></param>
        /// <param name="spriteBatch"></param>
        public TextRenderer(GraphicsDevice gd, VeldridSpriteBatch spriteBatch)
        {
            _sb = spriteBatch;
            _textRenderer = new(gd, _sb);
        }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            if (disposing)
            {

            }
        }

        /// <inheritdoc/>
        public void DrawString(IFont font, int fontSize, string text, Vector2 position, Color color, float roration, Vector2 origin, Vector2 scale, float layerDepth)
        {
            DrawString(Cast(font), fontSize, text, position, color, roration, origin, scale, layerDepth);
        }

        /// <inheritdoc/>
        public void DrawString(Font font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth)
        {
            var rf = font.FontSystem.GetFont(fontSize);
            rf.DrawText(_textRenderer, text, position, color, scale, rotation, origin, layerDepth);
        }

        private static Font Cast(IFont font)
        {
            if (font is not Font f)
                throw new InvalidCastException($"The {font} is not supported by this implementation.");
            return f;
        }

    }
}
