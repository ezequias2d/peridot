// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Drawing;

namespace Peridot;

/// <summary>
/// Represents a 2D texture to use in <see cref="SpriteBatch{TTexture}"/>.
/// </summary>
public interface Image : IDisposable
{
    /// <summary>
    /// A bool indicating whether this instance has been disposed.
    /// </summary>
    public bool IsDisposed { get; }

    /// <summary>
    /// The total size of the texture.
    /// </summary>
    public Size Size { get; }
    public PixelFormat Format { get; }

    public int Width => Size.Width;
    public int Height => Size.Height;
}
