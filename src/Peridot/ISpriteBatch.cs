// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Drawing;
using System.Numerics;

namespace Peridot
{
    /// <summary>
    /// Represents a interface for drawing sprites in one or more optimized batches.
    /// </summary>
    public interface ISpriteBatch : IDisposable
    {
        /// <summary>
        /// The view matrix to use to renderer.
        /// </summary>
        public Matrix4x4 ViewMatrix { get; set; }

        /// <summary>
        /// Begins the sprite branch.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Begin"/> is called next time without previous <see cref="End"/>.</exception>
        public void Begin();

        /// <summary>
        /// Flushes all batched text and sprites to the screen.
        /// </summary>
        /// <exception cref="InvalidOperationException">This command should be called after <see cref="Begin"/> and drawing commands.</exception>
        public void End();

        /// <summary>
        /// Submit sprite draw in the batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="origin">Sprite center.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
		public void Draw(ITexture2D texture,
                Rectangle destinationRectangle,
                Rectangle sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float layerDepth);

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
        public void Draw(ITexture2D texture,
                Vector2 position,
                Rectangle sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth);

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
		public void Draw(ITexture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            var srcRect = sourceRectangle ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture, destinationRectangle, srcRect, color, rotation, origin, layerDepth);
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
		public void Draw(ITexture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            var srcRect = sourceRectangle ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture, position, srcRect, color, rotation, origin, scale, layerDepth);
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
		public void Draw(ITexture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), layerDepth);
        }
        

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(ITexture2D texture,
            Vector2 position,
            Rectangle? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, position, sourceRectangle, color, 0f, default, 0f, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(ITexture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, default, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(ITexture2D texture,
            Vector2 position,
            Color color,
            float layerDepth)
        {
            Draw(texture, position, new Rectangle(default, texture.Size), color, 0, default, default, layerDepth);
        }

        /// <summary>
        /// Submit a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(ITexture2D texture,
            Rectangle destinationRectangle,
            Color color,
            float layerDepth)
        {
            Draw(texture, destinationRectangle, new Rectangle(default, texture.Size), color, 0, default, layerDepth);
        }
    }

    /// <summary>
    /// Represents a interface for drawing sprites in one or more optimized batches.
    /// </summary>
    public interface ISpriteBatch<TTexture> : ISpriteBatch where TTexture : notnull, ITexture2D
    {
        /// <summary>
        /// Submit sprite draw in the batch.
        /// </summary>
        /// <param name="texture">The source texture.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">Color multiplier.</param>
        /// <param name="rotation">The rotation of the sprite.</param>
        /// <param name="origin">Sprite center.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(TTexture texture,
                Rectangle destinationRectangle,
                Rectangle sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float layerDepth);

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
                Rectangle sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth);

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
            var srcRect = sourceRectangle ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture, destinationRectangle, srcRect, color, rotation, origin, layerDepth);
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
            var srcRect = sourceRectangle ?? new(0, 0, texture.Size.Width, texture.Size.Height);
            Draw(texture, position, srcRect, color, rotation, origin, scale, layerDepth);
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
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), layerDepth);
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
            Draw(texture, position, sourceRectangle, color, 0f, default, 0f, layerDepth);
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
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, default, layerDepth);
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
            Draw(texture, position, new Rectangle(default, texture.Size), color, 0, default, default, layerDepth);
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
            Draw(texture, destinationRectangle, new Rectangle(default, texture.Size), color, 0, default, layerDepth);
        }
    }
}
