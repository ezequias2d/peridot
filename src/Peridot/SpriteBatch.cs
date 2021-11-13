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
    public abstract class SpriteBatch<TTexture> : IDisposable where TTexture : notnull, ITexture2D
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
        }

        /// <summary>
        /// Deconstructor of <see cref="SpriteBatch{TTexture}"/>.
        /// </summary>
        ~SpriteBatch()
        {
            Dispose(false);
        }

        /// <summary>
        /// The view matrix to use to renderer.
        /// </summary>
        public Matrix4x4 ViewMatrix { get; set; }

        /// <summary>
        /// Begins the sprite branch.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
        public virtual void Begin()
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
        public virtual void End()
        {
            if (!_beginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");

            _beginCalled = false;
        }

        private ref BatchItem Draw(TTexture texture)
        {
            return ref _batcher.Add(texture);
        }

        /// <summary>
        /// Submit sprite draw in the batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="origin">Sprite center.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void Draw(TTexture texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);
            
            var size = texture.Size;
            var srcRect = sourceRectangle ?? new(0, 0, (int)size.X, (int)size.Y);
            item = new(size, position, srcRect, color, rotation, origin, scale, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Sprite center.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void Draw(TTexture texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                float layerDepth)
        {
            var scaleVec = new Vector2(scale, scale);
            Draw(texture, position, sourceRectangle, color, rotation, origin, scaleVec, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Sprite center.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void Draw(TTexture texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);

            var size = texture.Size;
            var srcRect = sourceRectangle ?? new(0, 0, (int)size.X, (int)size.Y);
            item = new(size, destinationRectangle, srcRect, color, rotation, origin, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(TTexture texture,
            Vector2 position, 
            Rectangle? sourceRectangle,
            Color color,
            float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);

            var size = texture.Size;
            var srcRect = sourceRectangle ?? new(0, 0, (int)size.X, (int)size.Y);
            item = new(size, position, srcRect, color, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(TTexture texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle, 
            Color color, 
            float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);

            var size = texture.Size;
            var srcRect = sourceRectangle ?? new(0, 0, (int)size.X, (int)size.Y);
            item = new(size, destinationRectangle, srcRect, color, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(TTexture texture, 
            Vector2 position, 
            Color color,
            float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);

            var size = texture.Size;
            item = new(size, position, color, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(TTexture texture, 
            Rectangle destinationRectangle, 
            Color color,
            float layerDepth)
        {
            CheckValid(texture);

            ref var item = ref Draw(texture);
            item = new(destinationRectangle, color, layerDepth);
        }

        /// <inheritdoc/>
        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        /// <param name="disposing">If called by <see cref="Dispose()"/></param>
        protected abstract void Dispose(bool disposing);

        private void CheckValid(TTexture texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));
            if (!_beginCalled)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        }
    }
}
