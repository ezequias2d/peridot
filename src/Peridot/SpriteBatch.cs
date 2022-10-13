// Copyright (c) 2021 ezequias2d <ezequiasmoises@gmail.com> and the Peridot contributors
// This code is licensed under MIT license (see LICENSE for details)

using System.Drawing;
using System.Numerics;

namespace Peridot;

/// <summary>
/// A class for drawing sprites in one or more optimized batches.
/// </summary>
/// <typeparam name="TImage">The image type to renderer.</typeparam>
public abstract class SpriteBatch<TImage> : ISpriteBatch<TImage>
{
    /// <summary>
    /// The batcher with all entities to renderer.
    /// </summary>
    protected readonly Batcher<TImage> m_batcher;

    protected readonly Image m_whiteImage;

    private bool m_beginCalled;

    /// <summary>
    /// Creates a new <see cref="SpriteBatch{TImage}"/>.
    /// </summary>
    public SpriteBatch(Image whiteImage)
    {
        m_batcher = new();
        m_whiteImage = whiteImage;
        m_beginCalled = false;
        IsDisposed = false;
        ResetScissor();
    }

    /// <summary>
    /// Deconstructor of <see cref="SpriteBatch{TImage}"/>.
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
        if (m_beginCalled)
            throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

        ViewMatrix = Matrix4x4.Identity;
        m_beginCalled = true;
        m_batcher.Clear();
    }

    /// <summary>
    /// Flushes all batched text and sprites to the screen.
    /// </summary>
    /// <exception cref="InvalidOperationException">This command should be called after <see cref="Begin"/> and drawing commands.</exception>
    public void End()
    {
        if (!m_beginCalled)
            throw new InvalidOperationException("Begin must be called before calling End.");

        m_beginCalled = false;
    }

    /// <inheritdoc/>
    public void Draw(Image image, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, SpriteOptions options, float layerDepth) =>
        Draw(GetHandle(image), destinationRectangle, sourceRectangle, color, rotation, origin, options, layerDepth);

    /// <inheritdoc/>
    public void Draw(Image image, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteOptions options, float layerDepth) =>
        Draw(GetHandle(image), position, sourceRectangle, color, rotation, origin, scale, options, layerDepth);

    /// <inheritdoc/>
    public void Draw(TImage image, RectangleF destinationRectangle, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, SpriteOptions options, float layerDepth)
    {
        CheckValid(image);
        ref var item = ref m_batcher.Add(image);
        var size = GetSize(image);
        var vsize = new Vector2(size.Width, size.Height);

        item = new(vsize, destinationRectangle, sourceRectangle, color, rotation, origin, layerDepth, Transform(Scissor, ViewMatrix), options);
    }

    /// <inheritdoc/>
    public void Draw(TImage image, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteOptions options, float layerDepth)
    {
        CheckValid(image);
        ref var item = ref m_batcher.Add(image);
        var size = GetSize(image);
        var vsize = new Vector2(size.Width, size.Height);

        item = new(vsize, position, sourceRectangle, color, rotation, origin, scale, layerDepth, Transform(Scissor, ViewMatrix), options);
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

    private void CheckValid(TImage image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));
        if (!m_beginCalled)
            throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
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
    public void Draw(Image image,
            RectangleF destinationRectangle,
            RectangleF sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
    {
        Draw(image, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
            Vector2 position,
            RectangleF sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth)
    {
        Draw(image, position, sourceRectangle, color, rotation, origin, scale, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteOptions options,
        float layerDepth)
    {
        var srcRect = sourceRectangle ?? new(0, 0, image.Width, image.Height);
        Draw(image, destinationRectangle, srcRect, color, rotation, origin, options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        float layerDepth)
    {
        Draw(image, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteOptions options,
            float layerDepth)
    {
        Draw(image: image,
            position: position,
            sourceRectangle: sourceRectangle ?? new()
            {
                X = 0,
                Y = 0,
                Width = image.Width,
                Height = image.Height,
            },
            color: color,
            rotation: rotation,
            origin: origin,
            scale: scale,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(Image image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteOptions options,
            float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(Image image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(Image image,
        Vector2 position,
        RectangleF? sourceRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(Image image,
        Vector2 position,
        RectangleF? sourceRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image: image,
            position: position,
            sourceRectangle: sourceRectangle,
            color: color,
            options: SpriteOptions.None,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image: image,
            destinationRectangle: destinationRectangle,
            sourceRectangle: sourceRectangle,
            color: color,
            rotation: 0,
            origin: default,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image: image,
            destinationRectangle:
            destinationRectangle,
            sourceRectangle: sourceRectangle,
            color: color,
            options: SpriteOptions.None,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        Vector2 position,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image: image,
            position: position,
            sourceRectangle: new Rectangle(default, image.Size),
            color: color,
            rotation: 0,
            origin: default,
            scale: default,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        Vector2 position,
        Color color,
        float layerDepth)
    {
        Draw(image: image,
            position: position,
            color: color,
            options: SpriteOptions.None,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image: image,
            destinationRectangle: destinationRectangle,
            sourceRectangle: new Rectangle(default, image.Size),
            color: color,
            rotation: 0,
            origin: default,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(Image image,
        RectangleF destinationRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image: image,
            destinationRectangle: destinationRectangle,
            color: color,
            options: SpriteOptions.None,
            layerDepth: layerDepth);
    }
    #endregion

    #region ISpriteBatch<TImage>

    /// <inheritdoc/>
    public void Draw(TImage image,
            RectangleF destinationRectangle,
            RectangleF sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float layerDepth)
    {
        Draw(image: image,
            destinationRectangle: destinationRectangle,
            sourceRectangle: sourceRectangle,
            color: color,
            rotation: rotation,
            origin: origin,
            options: SpriteOptions.None,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
            Vector2 position,
            RectangleF sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteOptions options,
        float layerDepth)
    {
        var size = GetSize(image);
        Draw(image: image,
            destinationRectangle: destinationRectangle,
            sourceRectangle: sourceRectangle ?? new()
            {
                X = 0,
                Y = 0,
                Width = size.Width,
                Height = size.Height,
            },
            color: color,
            rotation: rotation,
            origin: origin,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        float layerDepth)
    {
        Draw(image, destinationRectangle, sourceRectangle, color, rotation, origin, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            SpriteOptions options,
            float layerDepth)
    {
        var size = GetSize(image);
        Draw(image: image,
            position: position,
            sourceRectangle: sourceRectangle ?? new()
            {
                X = 0,
                Y = 0,
                Width = size.Width,
                Height = size.Height,
            },
            color: color,
            rotation: rotation,
            origin: origin,
            scale: scale,
            options: options,
            layerDepth: layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            Vector2 scale,
            float layerDepth)
    {
        Draw(image: image,
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
    public void Draw(TImage image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            SpriteOptions options,
            float layerDepth)
    {
        Draw(image, position, sourceRectangle, color, rotation, origin, new Vector2(scale), options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
            Vector2 position,
            RectangleF? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            float scale,
            float layerDepth)
    {
        Draw(image, position, sourceRectangle, color, rotation, origin, scale, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        Vector2 position,
        RectangleF? sourceRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image, position, sourceRectangle, color, 0f, default, 0f, options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        Vector2 position,
        RectangleF? sourceRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image, position, sourceRectangle, color, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image, destinationRectangle, sourceRectangle, color, 0, default, options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        RectangleF? sourceRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image, destinationRectangle, sourceRectangle, color, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        Vector2 position,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image, position, new Rectangle(default, GetSize(image)), color, 0, default, default, options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        Vector2 position,
        Color color,
        float layerDepth)
    {
        Draw(image, position, new Rectangle(default, GetSize(image)), color, 0, default, default, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        Color color,
        SpriteOptions options,
        float layerDepth)
    {
        Draw(image, destinationRectangle, new Rectangle(default, GetSize(image)), color, 0, default, options, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(TImage image,
        RectangleF destinationRectangle,
        Color color,
        float layerDepth)
    {
        Draw(image, destinationRectangle, color, SpriteOptions.None, layerDepth);
    }

    /// <inheritdoc/>
    public void DrawRect(RectangleF rectangle, Color color, float rotation, Vector2 origin, float layerDepth)
    {
        Draw(m_whiteImage, rectangle, new RectangleF(0, 0, 1, 1), color, rotation, origin, layerDepth);
    }

    /// <inheritdoc/>
    public void DrawRect(RectangleF rectangle, Color color, float layerDepth)
    {
        Draw(m_whiteImage, rectangle, new RectangleF(0, 0, 1, 1), color, layerDepth);
    }

    /// <inheritdoc/>
    public void DrawDot(Vector2 position, float size, Color color, float layerDepth)
    {
        Draw(m_whiteImage, new RectangleF(position.X, position.Y, size, size), new RectangleF(0, 0, 1, 1), color, layerDepth);
    }

    /// <inheritdoc/>
    public void DrawSegment(Vector2 a, Vector2 b, Color color, float thickness, float layerDepth)
    {
        var diff = b - a;
        var length = MathF.Sqrt(Vector2.Dot(diff, diff));
        var angle = MathF.Atan2(diff.Y, diff.X);
        DrawSegment(a, length, color, angle, thickness, layerDepth);
    }

    /// <inheritdoc/>
    public void DrawSegment(Vector2 start, float length, Color color, float angle, float thickness, float layerDepth)
    {
        var rect = new RectangleF(start.X, start.Y, length, thickness);
        DrawRect(rect, color, angle, new Vector2(0, 0.5f), layerDepth);
    }

    #endregion

    /// <summary>
    /// Gets size of a image.
    /// </summary>
    /// <param name="image">Image.</param>
    /// <returns>Image Size.</returns>
    protected abstract Size GetSize(TImage image);

    /// <summary>
    /// Gets TImage.
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    protected abstract TImage GetHandle(Image image);
}
