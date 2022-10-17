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
    /// Image size.
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Pixel format.
    /// </summary>
    /// <value></value>
    public PixelFormat Format { get; }

    /// <summary>
    /// Image width.
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    /// Image height.
    /// </summary>
    public int Height => Size.Height;
}
