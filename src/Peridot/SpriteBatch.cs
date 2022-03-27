// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

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
        public void Draw(ITexture2D texture, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, SpriteOptions options, float layerDepth) =>
            Draw(SpriteBatch<TTexture>.Cast(texture), destinationRectangle, sourceRectangle, color, rotation, origin, options, layerDepth);

        /// <inheritdoc/>
        public void Draw(ITexture2D texture, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteOptions options, float layerDepth) =>
            Draw(SpriteBatch<TTexture>.Cast(texture), position, sourceRectangle, color, rotation, origin, scale, options, layerDepth);

        /// <inheritdoc/>
        public void Draw(TTexture texture, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, SpriteOptions options, float layerDepth)
        {
            CheckValid(texture);
            ref var item = ref _batcher.Add(texture);

            var size = new Vector2(texture.Size.Width, texture.Size.Height);
            item = new(size, destinationRectangle, sourceRectangle, color, rotation, origin, layerDepth, Transform(Scissor, ViewMatrix), options);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteOptions options, float layerDepth)
        {
            CheckValid(texture);
            ref var item = ref _batcher.Add(texture);

            var size = new Vector2(texture.Size.Width, texture.Size.Height);
            item = new(size, position, sourceRectangle, color, rotation, origin, scale, layerDepth, Transform(Scissor, ViewMatrix), options);
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

        /// <inheritdoc/>
        public void IntersectScissor(RectangleF clip)
        {
            var scissor = Scissor;
            scissor.Intersect(clip);
            Scissor = scissor;
        }

        private static RectangleF Transform(RectangleF rect, Matrix4x4 matrix)
        {
            var pos = Vector4.Transform(new Vector4(rect.X, rect.Y, 0, 1), matrix);
            var size = Vector4.Transform(new Vector4(rect.X + rect.Width, rect.Y + rect.Height, 0, 1), matrix);
            return new(pos.X, pos.Y, size.X - pos.X, size.Y - pos.Y);
        }

        #region ISpriteBatch
        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
                RectangleF destinationRectangle,
                RectangleF sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
                Vector2 position,
                RectangleF sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteOptions options,
            float layerDepth)
        {
            var srcRect = sourceRectangle ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture, destinationRectangle, srcRect, color, rotation, origin, options, layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                SpriteOptions options,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle ?? new()
                {
                    X = 0,
                    Y = 0,
                    Width = texture.Size.Width,
                    Height = texture.Size.Height,
                },
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                SpriteOptions options,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: new Vector2(scale),
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(ITexture2D texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: 0f,
                origin: default,
                scale: 0f,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture: texture,
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: 0,
                origin: default,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture: texture,
                destinationRectangle:
                destinationRectangle,
                sourceRectangle: sourceRectangle,
                color: color,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            Vector2 position,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: new Rectangle(default, texture.Size),
                color: color,
                rotation: 0,
                origin: default,
                scale: default,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            Vector2 position,
            Color color,
            float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                color: color,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture: texture,
                destinationRectangle: destinationRectangle,
                sourceRectangle: new Rectangle(default, texture.Size),
                color: color,
                rotation: 0,
                origin: default,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(ITexture2D texture,
            RectangleF destinationRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture: texture,
                destinationRectangle: destinationRectangle,
                color: color,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }
        #endregion

        #region ISpriteBatch<TTexture>
        
        /// <inheritdoc/>
        public void Draw(TTexture texture,
                RectangleF destinationRectangle,
                RectangleF sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float layerDepth)
        {
            Draw(texture: texture,
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
                Vector2 position,
                RectangleF sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteOptions options,
            float layerDepth)
        {
            //var srcRect = sourceRectangleF ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture: texture,
                destinationRectangle: destinationRectangle,
                sourceRectangle: sourceRectangle ?? new()
                {
                    X = 0,
                    Y = 0,
                    Width = texture.Size.Width,
                    Height = texture.Size.Height,
                },
                color: color,
                rotation: rotation,
                origin: origin,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(TTexture texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                SpriteOptions options,
                float layerDepth)
        {
            //var srcRect = sourceRectangleF ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle ?? new()
                {
                    X = 0,
                    Y = 0,
                    Width = texture.Size.Width,
                    Height = texture.Size.Height,
                },
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: options,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(TTexture texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            Draw(texture: texture,
                position: position,
                sourceRectangle: sourceRectangle,
                color: color,
                rotation: rotation,
                origin: origin,
                scale: scale,
                options: SpriteOptions.None,
                layerDepth: layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                SpriteOptions options,
                float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), options, layerDepth);
        }

        /// <inheritdoc/>
		public void Draw(TTexture texture,
                Vector2 position,
                RectangleF? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, scale, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, 0f, default, 0f, options, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, default, options, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            RectangleF? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            Vector2 position,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture, position, new Rectangle(default, texture.Size), color, 0, default, default, options, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            Vector2 position,
            Color color,
            float layerDepth)
        {
            Draw(texture, position, new Rectangle(default, texture.Size), color, 0, default, default, SpriteOptions.None, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            Color color,
            SpriteOptions options,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, new Rectangle(default, texture.Size), color, 0, default, options, layerDepth);
        }

        /// <inheritdoc/>
        public void Draw(TTexture texture,
            RectangleF destinationRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, color, SpriteOptions.None, layerDepth);
        }
        #endregion
    }
}
