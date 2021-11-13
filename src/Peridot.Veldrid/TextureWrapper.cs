// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Veldrid;

namespace Peridot.Veldrid
{
    public struct TextureWrapper : ITexture2D, IEquatable<TextureWrapper>
    {
        public TextureWrapper(Texture texture)
        {
            Texture = texture;
            Size = new(texture.Width, texture.Height);
        }
        public Texture Texture { get; }
        public Vector2 Size { get; }

        public override int GetHashCode() => Texture.GetHashCode();

        public bool Equals(TextureWrapper other) => Texture == other.Texture;

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is not null && obj is TextureWrapper tw && Equals(tw);

        public override string ToString() => Texture.ToString();

        public static implicit operator TextureWrapper(Texture t) => new(t);
        public static implicit operator Texture(TextureWrapper t) => t.Texture;
    }
}
