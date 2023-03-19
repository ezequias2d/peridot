using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Peridot;

/// <summary>
/// Provides static methods to <see cref="ColorF"/> manipulation.
/// </summary>
public partial struct ColorF
{
    #region Cast operators
    /// <summary>
    /// Cast a <see cref="ColorF"/> to <see cref="Vector4"/>.
    /// </summary>
    /// <param name="color">The color to cast.</param>
    public static implicit operator Vector4(ColorF color) => color.m_rgba;

    /// <summary>
    /// Cast a <see cref="Vector4"/> to <see cref="ColorF"/>.
    /// </summary>
    /// <param name="color"></param>
    public static implicit operator ColorF(Vector4 color) => new(color);

    /// <summary>
    /// Cast a <see cref="Vector4"/> to <see cref="ColorF"/>.
    /// </summary>
    /// <param name="color"></param>
    public static implicit operator Color(ColorF color)
    {
        color = Vector4.Clamp(color, Vector4.Zero, Vector4.One) * 255f;
        return Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
    }

    /// <summary>
    /// Cast a <see cref="Vector4"/> to <see cref="ColorF"/>.
    /// </summary>
    /// <param name="color"></param>
    public static implicit operator ColorF(Color color)
    {
        return (ColorF)Vector4.Clamp(new Vector4(color.R, color.G, color.B, color.A) * (1f / 255f), Vector4.Zero, Vector4.One);
    }
    #endregion
    #region Math
    /// <summary>
    /// Negates the specified color.
    /// </summary>
    /// <param name="color">The color to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Negate(ColorF color) => -color.m_rgba;

    /// <summary>
    /// Adds two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The summed color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Add(ColorF left, ColorF right) => left.m_rgba + right.m_rgba;

    /// <summary>
    /// Subtracts the second color from the first.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from subracting <paramref name="right"/> from <paramref name="left"/>.</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Subtract(ColorF left, ColorF right) => left.m_rgba - right.m_rgba;

    /// <summary>
    /// Multiplies two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The product color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Multiply(ColorF left, ColorF right) => left.m_rgba * right.m_rgba;

    /// <summary>
    /// Divides the first color by the second.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from dividing <paramref name="left"/> by <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Divide(ColorF left, ColorF right) => left.m_rgba / right.m_rgba;

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Multiply(ColorF left, float right) => left.m_rgba * right;

    /// <summary>
    /// Divides the specified color by a specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Divide(ColorF left, float right) => left.m_rgba / right;

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The scalar value.</param>
    /// <param name="right">The color.</param>
    /// <returns>The scaled color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Multiply(float left, ColorF right) => left * right.m_rgba;

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    /// <param name="a">The first color.</param>
    /// <param name="b">The second color.</param>
    /// <param name="t">Influence of the second color on the final result.</param>
    /// <returns><paramref name="a"/> * (1f - <paramref name="t"/>) + <paramref name="b"/> * <paramref name="t"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorF Lerp(in ColorF a, in ColorF b, float t) =>
        a + (b - a) * t;
    #endregion
    #region Operators
    /// <summary>
    /// Element-wise equality.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ColorF left, ColorF right) =>
        left.Equals(right);

    /// <summary>
    /// Element-wise inequality.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ColorF left, ColorF right) =>
        !left.Equals(right);

    /// <summary>
    /// Negates the specified color.
    /// </summary>
    /// <param name="color">The color to negate.</param>
    /// <returns>The negated vector.</returns>
    public static ColorF operator -(ColorF color) => Negate(color);

    /// <summary>
    /// Adds two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The summed color.</returns>
    public static ColorF operator +(ColorF left, ColorF right) => Add(left, right);

    /// <summary>
    /// Subtracts the second color from the first.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from subracting <paramref name="right"/> from <paramref name="left"/>.</returns>
    public static ColorF operator -(ColorF left, ColorF right) => Subtract(left, right);

    /// <summary>
    /// Multiplies two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The product color.</returns>
    public static ColorF operator *(ColorF left, ColorF right) => Multiply(left, right);

    /// <summary>
    /// Divides the first color by the second.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from dividing <paramref name="left"/> by <paramref name="right"/>.</returns>
    public static ColorF operator /(ColorF left, ColorF right) => Divide(left, right);

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled color.</returns>
    public static ColorF operator *(ColorF left, float right) => Multiply(left, right);

    /// <summary>
    /// Divides the specified color by a specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    public static ColorF operator /(ColorF left, float right) => Divide(left, right);

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The scalar value.</param>
    /// <param name="right">The color.</param>
    /// <returns>The scaled color.</returns>
    public static ColorF operator *(float left, ColorF right) => Multiply(left, right);
    #endregion
    #region Colors
    /// <summary>
    /// Red (1, 0, 0, 1)
    /// </summary>
    public static readonly ColorF Red = new(1f, 0f, 0f, 1f);

    /// <summary>
    /// Dark Red (139f / 255f, 0f, 0f, 1f)
    /// </summary>
    public static readonly ColorF DarkRed = new(139f / 255f, 0f, 0f, 1f);

    /// <summary>
    /// Green (0f, 1f, 0f, 1f)
    /// </summary>
    public static readonly ColorF Green = new(0f, 1f, 0f, 1f);

    /// <summary>
    /// Blue (0f, 0f, 1f, 1f)
    /// </summary>
    public static readonly ColorF Blue = new(0f, 0f, 1f, 1f);

    /// <summary>
    /// Yellow (1f, 1f, 0f, 1f)
    /// </summary>
    public static readonly ColorF Yellow = new(1f, 1f, 0f, 1f);

    /// <summary>
    /// Grey (128f / 255f, 128f / 255f, 128 / 255f, 1f)
    /// </summary>
    public static readonly ColorF Grey = new(128f / 255f, 128f / 255f, 128f / 255f, 1f);

    /// <summary>
    /// Light Grey (211f / 255f, 211f / 255f, 211f / 255f, 1f)
    /// </summary>
    public static readonly ColorF LightGrey = new(211f / 255f, 211f / 255f, 211f / 255f, 1f);

    /// <summary>
    /// Cyan (0f, 1f, 1f, 1f)
    /// </summary>
    public static readonly ColorF Cyan = new(0f, 1f, 1f, 1f);

    /// <summary>
    /// White (1f, 1f, 1f, 1f)
    /// </summary>
    public static readonly ColorF White = new(1f, 1f, 1f, 1f);

    /// <summary>
    /// Cornflower Blue (100f / 255f, 149f / 255f, 237f / 255f, 1f)
    /// </summary>
    public static readonly ColorF CornflowerBlue = new(100f / 255f, 149f / 255f, 237f / 255f, 1f);

    /// <summary>
    /// Clear (0f, 0f, 0f, 0f)
    /// </summary>
    public static readonly ColorF Clear = new(0f, 0f, 0f, 0f);

    /// <summary>
    /// Black (0f, 0f, 0f, 1f)
    /// </summary>
    public static readonly ColorF Black = new(0f, 0f, 0f, 1f);

    /// <summary>
    /// Pink (1f, 192f / 255f, 203f / 255f, 1f)
    /// </summary>
    public static readonly ColorF Pink = new(1f, 192f / 255f, 203f / 255f, 1f);

    /// <summary>
    /// Orange (1f, 165f / 255f, 0f, 1f)
    /// </summary>
    public static readonly ColorF Orange = new(1f, 165f / 255f, 0f, 1f);
    #endregion
}
