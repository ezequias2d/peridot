using FontStashSharp.Interfaces;
using System.Drawing;
using System.Numerics;
using Veldrid;

namespace Peridot.Veldrid
{
    internal class FontStashRenderer : IFontStashRenderer, IDisposable
    {
        private bool _disposed;
        private readonly VeldridSpriteBatch _sb;
        private readonly Texture2DManager _texture2DManager;
        public FontStashRenderer(GraphicsDevice gd, VeldridSpriteBatch sb)
        {
            _texture2DManager = new Texture2DManager(gd);
            _sb = sb;
            ResetScissor();
        }

        ~FontStashRenderer()
        {
            Dispose(false);
        }

        public Rectangle Scissor { get; set; }

        public ITexture2DManager TextureManager => _texture2DManager;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            if (disposing)
                _texture2DManager.Dispose();
        }

        public void ResetScissor()
        {
            const int v = 1 << 23;
            const int s = -(1 << 22);
            Scissor = new Rectangle(v, v, s, s);
        }

        public void Draw(object texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 origin, Vector2 scale, float depth)
        {
            if (texture is not Texture t)
                throw new ArgumentException(nameof(texture));

            var s = src ?? new(0, 0, (int)t.Width, (int)t.Height);

            var oldScissor = _sb.Scissor;
            _sb.Scissor = RectangleF.Intersect(_sb.Scissor, Scissor);
            _sb.Draw(t, pos, s, color, rotation, origin, new Vector2(scale.X, scale.Y), depth);
            _sb.Scissor = oldScissor;
        }
    }
}
