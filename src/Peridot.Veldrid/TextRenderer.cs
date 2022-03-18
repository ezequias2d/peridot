// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

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
        ~TextRenderer()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
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
        public void DrawString(IFont font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth)
        {
            DrawString(Cast(font), fontSize, text, position, color, rotation, origin, scale, layerDepth);
        }

        /// <inheritdoc/>
        public void DrawString(IFont font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth, Rectangle scissor)
        {
            DrawString(Cast(font), fontSize, text, position, color, rotation, origin, scale, layerDepth, scissor);
        }

        /// <inheritdoc/>
        public void DrawString(Font font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth)
        {
            var rf = font.FontSystem.GetFont(fontSize);
            rf.DrawText(_textRenderer, text, position, color, scale, rotation, origin, layerDepth);
        }

        /// <inheritdoc/>
        public void DrawString(Font font, int fontSize, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth, Rectangle scissor)
        {
            var rf = font.FontSystem.GetFont(fontSize);
            _textRenderer.Scissor = scissor;
            rf.DrawText(_textRenderer, text, position, color, scale, rotation, origin, layerDepth);
            _textRenderer.ResetScissor();
        }

        private static Font Cast(IFont font)
        {
            if (font is not Font f)
                throw new InvalidCastException($"The {font} is not supported by this implementation.");
            return f;
        }
    }
}
