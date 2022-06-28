// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Veldrid;

namespace Peridot.Veldrid;

/// <summary>
/// A wrapper with <see cref="ITexture2D"/> interface.
/// </summary>
public class TextureWrapper : ITexture2D, IEquatable<TextureWrapper>
{
    /// <summary>
    /// Creates a new instance of <see cref="TextureWrapper"/>.
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public TextureWrapper(Texture texture)
    {
        Texture = texture ?? throw new ArgumentNullException(nameof(texture));
    }

    /// <summary>
    /// Texture
    /// </summary>
    public Texture Texture { get; }

    /// <summary>
    /// Size of texture
    /// </summary>
    public Size Size => new((int)Texture.Width, (int)Texture.Height);

    /// <inheritdoc/>
    public bool IsDisposed => Texture.IsDisposed;

    /// <inheritdoc/>
    public override int GetHashCode() => Texture.GetHashCode();

    /// <inheritdoc/>
    public bool Equals(TextureWrapper? other) => Texture == other?.Texture;

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is TextureWrapper tw && Equals(tw);

    /// <inheritdoc/>
    public override string? ToString() => Texture.ToString();

    /// <inheritdoc/>
    public void Dispose()
    {
        Texture.Dispose();
        GC.SuppressFinalize(this);
    }

    public static implicit operator TextureWrapper(Texture t) => new(t);

    public static implicit operator Texture(TextureWrapper t) => t.Texture;
    
    public static bool operator ==(TextureWrapper left, TextureWrapper right) => left.Equals(right);

    public static bool operator !=(TextureWrapper left, TextureWrapper right) => !(left == right);
}
