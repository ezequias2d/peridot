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

            UV = CreateUV(options, sourceSize, pos);
            Color = ToVector(color);
            Scale = destinationRectangle.Size.ToVector2();
            Origin = origin;
            Location = new(destinationRectangle.Location.ToVector2(), layerDepth);
            Rotation = rotation;
            Scissor = scissor;
        }

        public BatchItem(Vector2 textureSize, Vector2 position, RectangleF sourceRectangle, Color color, float rotation,
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

        public Vector4 UV { get; set; }
        public Vector4 Color { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; set; }
        public Vector3 Location { get; set; }
        public float Rotation { get; set; }
        public RectangleF Scissor { get; set; }

        private static Vector4 CreateUV(SpriteOptions options, Vector2 sourceSize, Vector2 sourceLocation)
        {
            if (options != SpriteOptions.None)
            {
                // flipX
                if (options.HasFlag(SpriteOptions.FlipHorizontally))
                {
                    sourceLocation.X += sourceSize.X;
                    sourceSize.X *= -1;
                }

                // flipY
                if (options.HasFlag(SpriteOptions.FlipVertically))
                {
                    sourceLocation.Y += sourceSize.Y;
                    sourceSize.Y *= -1;
                }
            }

            return new(sourceLocation.X, sourceLocation.Y, sourceSize.X, sourceSize.Y);
        }

        private static Vector4 ToVector(Color color) => new Vector4(color.R, color.G, color.B, color.A) * (1f / 255f);

        public override string ToString()
        {
            return $"uv: {UV}, color: {Color}, scale: {Scale}, origin: {Origin}, location: {new Vector4(Location, Rotation)}, scissor: {Scissor}";
        }
    }
}
