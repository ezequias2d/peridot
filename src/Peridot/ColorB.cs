using System.Drawing;

namespace Peridot;

/// <summary>
/// Represetns a RGBA color with floating point values.
/// All components must be between 0.0 and 1.0.
/// </summary>
public partial struct ColorB : IEquatable<ColorB>
{
    /// <summary>
    /// Constructs a new ColorB from components.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    public ColorB(byte r, byte g, byte b, byte a)
    {
        (R, G, B, A) = (r, g, b, a);
    }

    /// <summary>
    /// Constructs a new ColorB from a Color.
    /// </summary>
    /// <param name="color">Color.</param>
    public ColorB(Color color)
    {
        (R, G, B, A) = (color.R, color.G, color.B, color.A);
    }

    /// <summary>
    /// Constructs a new ColorB from a Vector4.
    /// </summary>
    /// <param name="color">Color.</param>
    public ColorB(ColorF color)
    {
        color *= 255f;
        (R, G, B, A) = ((byte)color.R, (byte)color.G, (byte)color.B, (byte)color.A);
    }

    /// <summary>
    /// The red color component.
    /// </summary>
    public byte R;

    /// <summary>
    /// The green color component.
    /// </summary>
    public byte G;

    /// <summary>
    /// The blue color component.
    /// </summary>
    public byte B;

    /// <summary>
    /// The alpha color component.
    /// </summary>
    public byte A;

    /// <inheritdoc/>
    public bool Equals(ColorB other)
    {
        return (R, G, B, A) == (other.R, other.G, other.B, other.A);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj != null && obj is ColorB other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{R: {R}, G: {G}, B: {B}, A: {A}}}";
    }
}
