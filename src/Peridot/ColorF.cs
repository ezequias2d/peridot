using System.Drawing;
using System.Numerics;

namespace Peridot;

/// <summary>
/// Represetns a RGBA color with floating point values.
/// All components must be between 0.0 and 1.0.
/// </summary>
public partial struct ColorF : IEquatable<ColorF>
{
    private Vector4 m_rgba;

    /// <summary>
    /// Constructs a new ColorF from components.
    /// </summary>
    /// <param name="r">Red component.</param>
    /// <param name="g">Green component.</param>
    /// <param name="b">Blue component.</param>
    /// <param name="a">Alpha component.</param>
    public ColorF(float r, float g, float b, float a)
    {
        m_rgba = new(r, g, b, a);
    }

    /// <summary>
    /// Constructs a new ColorF from a Color.
    /// </summary>
    /// <param name="color">Color.</param>
    public ColorF(Color color)
    {
        m_rgba = new Vector4(color.R, color.G, color.B, color.A) * (1f / 255f);
    }

    /// <summary>
    /// Constructs a new ColorF from a Vector4.
    /// </summary>
    /// <param name="color">Color.</param>
    public ColorF(Vector4 color)
    {
        m_rgba = color;
    }

    /// <summary>
    /// The red color component.
    /// </summary>
    public float R { get => m_rgba.X; set => m_rgba.X = value; }

    /// <summary>
    /// The green color component.
    /// </summary>
    public float G { get => m_rgba.Y; set => m_rgba.Y = value; }

    /// <summary>
    /// The blue color component.
    /// </summary>
    public float B { get => m_rgba.Z; set => m_rgba.Z = value; }

    /// <summary>
    /// The alpha color component.
    /// </summary>
    public float A { get => m_rgba.W; set => m_rgba.W = value; }

    public Vector4 AsVector { get => m_rgba; set => m_rgba = value; }

    public bool Equals(ColorF other)
    {
        return m_rgba.Equals(other.m_rgba);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is ColorF other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return m_rgba.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{{R: {R}, G: {G}, B: {B}, A: {A}}}";
    }
}
