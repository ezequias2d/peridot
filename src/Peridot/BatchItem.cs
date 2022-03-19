// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Drawing;
using System.Numerics;

namespace Peridot
{
    public struct BatchItem
    {
        public BatchItem(Vector2 textureSize, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color,
                         float rotation, Vector2 origin, float layerDepth, RectangleF scissor, SpriteOptions options)
        {
            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y) / textureSize;

            UV = CreateFlip(options) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1)) * Matrix4x4.CreateTranslation(new(pos, 0));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(destinationRectangle.Width, destinationRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(-origin, 0)) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(new Vector3(destinationRectangle.X, destinationRectangle.Y, layerDepth));
            Projection = Matrix4x4.Identity;
            Scissor = scissor;
        }

        public BatchItem(Vector2 textureSize, Vector2 position, Rectangle sourceRectangle, Color color, float rotation,
                         Vector2 origin, Vector2 scale, float layerDepth, RectangleF scissor, SpriteOptions options) 
            : this(textureSize, new RectangleF(position.X, position.Y, sourceRectangle.Width * scale.X, sourceRectangle.Height * scale.Y), 
                    sourceRectangle, 
                    color, 
                    rotation,
                    origin, 
                    layerDepth,
                    scissor, 
                    options)
        {
        }

        public Matrix4x4 UV { get; set; }
        public Vector4 Color { get; set; }
        public Matrix4x4 Model { get; set; }
        public Matrix4x4 Projection { get; set; }
        public RectangleF Scissor { get; set; }

        private static Vector4 ToVector(Color color) => new Vector4(color.R, color.G, color.B, color.A) * (1f / 255f);

        private static Matrix4x4 CreateFlip(SpriteOptions options)
        {
            if (options == SpriteOptions.None)
                return Matrix4x4.Identity;

            var flipX = options.HasFlag(SpriteOptions.FlipHorizontally);
            var flipY = options.HasFlag(SpriteOptions.FlipVertically);

            return Matrix4x4.CreateScale(flipX ? -1 : 1, flipY ? -1 : 1, 1) * Matrix4x4.CreateTranslation(flipX ? 1 : 0, flipY ? 1 : 0, 0);
        }
    }
}
