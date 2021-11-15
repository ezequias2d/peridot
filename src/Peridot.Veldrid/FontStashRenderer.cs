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
        }

        ~FontStashRenderer()
        {
            Dispose(false);
        }

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

        public void Draw(object texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 origin, Vector2 scale, float depth)
        {
            if (texture is not Texture t)
                throw new ArgumentException(nameof(texture));

            Rectangle source = src ?? new(0, 0, (int)t.Width, (int)t.Height);
            _sb.Draw(t, pos, source, color, rotation, origin, new Vector2(scale.X, scale.Y), depth);
        }
    }
}
