// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Drawing;
using System.Numerics;

namespace Peridot.Text
{
    /// <summary>
    /// Represents an interface to a text renderer.
    /// </summary>
    public class TextRenderer : IDisposable
    {
        private readonly ISpriteBatch m_spriteBatch;
        private readonly FontStashRenderer _textRenderer;

        /// <summary>
        /// Creates a new instance of <see cref="TextRenderer"/>.
        /// </summary>
        /// <param name="peridot">Peridot implementation.</param>
        /// <param name="spriteBatch">SpriteBatch to use to render.</param>
        public TextRenderer(IPeridot peridot, ISpriteBatch spriteBatch)
        {
            m_spriteBatch = spriteBatch;
            _textRenderer = new(peridot, spriteBatch);
        }

        /// <inheritdoc/>
        ~TextRenderer()
        {
            Dispose(false);
        }

        /// <summary>
        /// A bool indicating whether this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

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
            Font font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth)
        {
            var rf = font.FontSystem.GetFont(fontSize);
            var fcolor = new FontStashSharp.FSColor(color.R, color.G, color.B, color.A);

            rf.DrawText(_textRenderer, text, position, fcolor, scale, rotation, origin, layerDepth);
        }

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
        /// <param name="scissor">The scissor rectangle.</param>
        public void DrawString(
            Font font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth,
            Rectangle scissor)
        {
            var rf = font.FontSystem.GetFont(fontSize);
            var fcolor = new FontStashSharp.FSColor(color.R, color.G, color.B, color.A);

            _textRenderer.Scissor = scissor;
            rf.DrawText(_textRenderer, text, position, fcolor, scale, rotation, origin, layerDepth);
            _textRenderer.ResetScissor();
        }

        /// <summary>
        /// Submit a text string for drawing.
        /// </summary>
        /// <param name="font">The font to draw.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="text">The text which will be drawn.</param>
        /// <param name="position">The drawing location on screen</param>
        /// <param name="color">The text color.</param>
        public void DrawString(
            Font font,
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
        /// <param name="scissor">The scissor rectangle.</param>
        public void DrawString(
            Font font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            Rectangle scissor)
        {
            DrawString(font, fontSize, text, position, color, 0, Vector2.Zero, Vector2.One, 0, scissor);
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
            Font font,
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
        /// <param name="scissor">The scissor rectangle.</param>
        public void DrawString(
            Font font,
            int fontSize,
            string text,
            Vector2 position,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            float layerDepth,
            Rectangle scissor)
        {
            DrawString(font, fontSize, text, position, color, rotation, origin, new Vector2(scale), layerDepth, scissor);
        }

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
    }
}
