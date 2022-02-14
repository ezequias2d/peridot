// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using FontStashSharp.Interfaces;
using System.Drawing;
using Veldrid;

namespace Peridot.Veldrid
{
    internal class Texture2DManager : ITexture2DManager, IDisposable
    {
        private readonly GraphicsDevice _gd;
        private readonly List<WeakReference<Texture>> _textures;
        private bool _disposed;
        public Texture2DManager(GraphicsDevice gd)
        {
            _gd = gd;
            _textures = new();
        }

        ~Texture2DManager()
        {
            Dispose(false);
        }

        public object CreateTexture(int width, int height)
        {
            var texture = _gd.ResourceFactory.CreateTexture(
                new(
                    (uint)width, (uint)height, 1,
                    1, 1,
                    PixelFormat.B8_G8_R8_A8_UNorm,
                    TextureUsage.Sampled,
                    TextureType.Texture2D));

            _textures.Add(new(texture));
            return texture;
        }

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
            {
                foreach (var wt in _textures)
                    if (wt.TryGetTarget(out var texture))
                        texture.Dispose();
            }
        }

        public Point GetTextureSize(object texture)
        {
            var t = texture as Texture;
            if (t is null)
                return Point.Empty;

            return new((int)t.Width, (int)t.Height);
        }

        public void SetTextureData(object texture, Rectangle bounds, byte[] data)
        {
            if (texture is not Texture t)
                return;

            _gd.UpdateTexture(t, data, (uint)bounds.X, (uint)bounds.Y, 0, (uint)bounds.Width, (uint)bounds.Height, 1, 0, 0);
        }
    }
}
