// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Drawing;
using System.Numerics;

namespace Peridot
{
    public struct BatchItem
    {
        public BatchItem(RectangleF uvRectangle, RectangleF destination, float rotation = 0, Vector2 origin = default, float depth = 0)
        {
            UV = ToMatrix(uvRectangle);
            Color = new(1f);
            Model = ToMatrix(destination, rotation, origin) * Matrix4x4.CreateTranslation(0, 0, depth);
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(
                Vector2 textureSize,
                Vector2 position,
                Rectangle sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                float layerDepth)
        {
            origin *= scale;

            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y);

            UV = Matrix4x4.CreateTranslation(new(pos, 0)) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(textureSize * scale, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(-origin, 0)) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(new Vector3(position, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(Vector2 textureSize, 
            Rectangle destinationRectangle,
            Rectangle sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
        {
            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y);

            UV = Matrix4x4.CreateTranslation(new(pos, 0)) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(destinationRectangle.Width, destinationRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(-origin, 0)) *
                Matrix4x4.CreateRotationZ(rotation) *
                Matrix4x4.CreateTranslation(new Vector3(destinationRectangle.X, destinationRectangle.Y, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(Vector2 textureSize,
            Vector2 position,
            Rectangle sourceRectangle,
            Color color,
            float layerDepth)
        {
            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y);

            UV = Matrix4x4.CreateTranslation(new(pos, 0)) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(sourceRectangle.Width, sourceRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(Vector2 textureSize,
            Rectangle destinationRectangle,
            Rectangle sourceRectangle,
            Color color,
            float layerDepth)
        {
            var sourceSize = new Vector2(sourceRectangle.Width, sourceRectangle.Height) / textureSize;
            var pos = new Vector2(sourceRectangle.X, sourceRectangle.Y);

            UV = Matrix4x4.CreateTranslation(new(pos, 0)) * Matrix4x4.CreateScale(new Vector3(sourceSize, 1));
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(destinationRectangle.Width, destinationRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(destinationRectangle.X, destinationRectangle.Y, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(Vector2 textureSize,
            Vector2 position,
            Color color,
            float layerDepth)
        {
            UV = Matrix4x4.Identity;
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(textureSize.X, textureSize.Y, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public BatchItem(Rectangle destinationRectangle,
            Color color,
            float layerDepth)
        {
            UV = Matrix4x4.Identity;
            Color = ToVector(color);
            Model =
                Matrix4x4.CreateScale(new Vector3(destinationRectangle.Width, destinationRectangle.Height, 0)) *
                Matrix4x4.CreateTranslation(new Vector3(destinationRectangle.X, destinationRectangle.Y, layerDepth));
            Projection = Matrix4x4.Identity;
        }

        public Matrix4x4 UV { get; set; }
        public Vector4 Color { get; set; }
        public Matrix4x4 Model { get; set; }
        public Matrix4x4 Projection { get; set; }

        private static Matrix4x4 ToMatrix(RectangleF r) =>
            Matrix4x4.CreateScale(new Vector3(r.Width, r.Height, 1)) *
            Matrix4x4.CreateTranslation(new(r.X, r.Y, 0));

        private static Matrix4x4 ToMatrix(RectangleF r, float rotation, Vector2 origin) =>
            Matrix4x4.CreateScale(new Vector3(r.Width, r.Height, 1)) *
            Matrix4x4.CreateTranslation(new(-origin, 0)) *
            Matrix4x4.CreateRotationZ(rotation) *
            Matrix4x4.CreateTranslation(new(r.X, r.Y, 0));

        private static Vector4 ToVector(Color color) => new Vector4(color.R, color.G, color.B, color.A) * (1f / 255f);
    }
}
