// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Drawing;
using System.Numerics;

namespace Peridot
{
    /// <summary>
    /// A class for drawing sprites in one or more optimized batches.
    /// </summary>
    /// <typeparam name="TTexture">The texture type to renderer.</typeparam>
    public abstract class SpriteBatch<TTexture> : ISpriteBatch<TTexture> where TTexture : notnull, ITexture2D
    {
        /// <summary>
        /// The batcher with all entities to renderer.
        /// </summary>
        protected readonly Batcher<TTexture> _batcher;

        private bool _beginCalled;

        /// <summary>
        /// Creates a new <see cref="SpriteBatch{TTexture}"/>.
        /// </summary>
        public SpriteBatch()
        {
            _batcher = new();
            _beginCalled = false;
            IsDisposed = false;
            ResetScissor();
        }

        /// <summary>
        /// Deconstructor of <see cref="SpriteBatch{TTexture}"/>.
        /// </summary>
        ~SpriteBatch()
        {
            CoreDispose(false);
        }

        /// <summary>
        /// The view matrix to use to renderer.
        /// </summary>
        public Matrix4x4 ViewMatrix { get; set; }

        /// <inheritdoc/>
        public bool IsDisposed { get; protected set; }

        /// <inheritdoc/>
        public RectangleF Scissor { get; set; }

        /// <summary>
        /// Begins the sprite branch.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
        public void Begin()
        {
            if (_beginCalled)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

            ViewMatrix = Matrix4x4.Identity;
            _beginCalled = true;
            _batcher.Clear();
        }

        /// <summary>
        /// Flushes all batched text and sprites to the screen.
        /// </summary>
        /// <exception cref="InvalidOperationException">This command should be called after <see cref="Begin"/> and drawing commands.</exception>
        public void End()
        {
            if (!_beginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");

            _beginCalled = false;
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, float layerDepth) =>
            Draw(SpriteBatch<TTexture>.Cast(texture), destinationRectangle, sourceRectangle, color, rotation, origin, layerDepth);

        /// <inheritdoc/>
        public void Draw(ITexture2D texture, Vector2 position, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth) =>
            Draw(SpriteBatch<TTexture>.Cast(texture), position, sourceRectangle, color, rotation, origin, scale, layerDepth);

        /// <inheritdoc/>
        public void Draw(TTexture texture, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, float layerDepth)
        {
            CheckValid(texture);
            ref var item = ref _batcher.Add(texture);

            var size = new Vector2(texture.Size.Width, texture.Size.Height);
            item = new(size, destinationRectangle, sourceRectangle, color, rotation, origin, layerDepth, Transform(Scissor, ViewMatrix));

        }

        /// <inheritdoc/>
        public void Draw(TTexture texture, Vector2 position, Rectangle sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth)
        {
            CheckValid(texture);
            ref var item = ref _batcher.Add(texture);

            var size = new Vector2(texture.Size.Width, texture.Size.Height);
            item = new(size, position, sourceRectangle, color, rotation, origin, scale, layerDepth, Transform(Scissor, ViewMatrix));
        }

        /// <inheritdoc/>
        public void Dispose() 
        {
            CoreDispose(true);
            GC.SuppressFinalize(this);
        }

        private void CoreDispose(bool disposing)
        {
            if (IsDisposed)
                return;
            IsDisposed = true;

            Dispose(disposing);
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing">If called by <see cref="Dispose()"/></param>
        protected abstract void Dispose(bool disposing);

        private void CheckValid(ITexture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            if (!_beginCalled)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        }

        private static TTexture Cast(ITexture2D texture)
        {
            if (texture is not TTexture tt)
                throw new InvalidCastException($"The {texture} is not supported by this implementation.");
            return tt;
        }

        /// <inheritdoc/>
        public void ResetScissor()
        {
            const float v = 1 << 23;
            const float s = -(1 << 22);
            Scissor = new RectangleF(s, s, v, v);
        }

        private static RectangleF Transform(RectangleF rect, Matrix4x4 matrix)
        {
            var pos = Vector4.Transform(new Vector4(rect.X, rect.Y, 0, 0), matrix);
            var size = Vector4.Transform(new Vector4(rect.X + rect.Width, rect.Y + rect.Height, 0, 0), matrix);
            return new(pos.X, pos.Y, size.X - pos.X, size.Y - pos.Y);
        }
    }
}
