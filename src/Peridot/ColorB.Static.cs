using System.Runtime.CompilerServices;

namespace Peridot;

/// <summary>
/// Provides static methods to <see cref="ColorB"/> manipulation.
/// </summary>
public partial struct ColorB
{
    #region Math
    /// <summary>
    /// Negates the specified color.
    /// </summary>
    /// <param name="color">The color to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Negate(ColorB color)
    {
        var r = -color.R;
        var g = -color.G;
        var b = -color.B;
        var a = -color.A;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Adds two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The summed color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Add(ColorB left, ColorB right)
    {
        var r = left.R + right.R;
        var g = left.G + right.G;
        var b = left.B + right.B;
        var a = left.A + right.A;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Subtracts the second color from the first.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from subracting <paramref name="right"/> from <paramref name="left"/>.</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Subtract(ColorB left, ColorB right)
    {
        var r = left.R - right.R;
        var g = left.G - right.G;
        var b = left.B - right.B;
        var a = left.A - right.A;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Multiplies two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The product color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Multiply(ColorB left, ColorB right)
    {
        var r = left.R * right.R;
        var g = left.G * right.G;
        var b = left.B * right.B;
        var a = left.A * right.A;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Divides the first color by the second.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from dividing <paramref name="left"/> by <paramref name="right"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Divide(ColorB left, ColorB right)
    {
        var r = left.R / right.R;
        var g = left.G / right.G;
        var b = left.B / right.B;
        var a = left.A / right.A;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Multiply(ColorB left, float right)
    {
        var r = left.R * right;
        var g = left.G * right;
        var b = left.B * right;
        var a = left.A * right;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Divides the specified color by a specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Divide(ColorB left, float right)
    {
        var r = left.R / right;
        var g = left.G / right;
        var b = left.B / right;
        var a = left.A / right;

        return new((byte)r, (byte)g, (byte)b, (byte)a);
    }

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The scalar value.</param>
    /// <param name="right">The color.</param>
    /// <returns>The scaled color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Multiply(float left, ColorB right) => Multiply(right, left);

    /// <summary>
    /// Linearly interpolates between two colors.
    /// </summary>
    /// <param name="a">The first color.</param>
    /// <param name="b">The second color.</param>
    /// <param name="t">Influence of the second color on the final result.</param>
    /// <returns><paramref name="a"/> * (255 - <paramref name="t"/>) + <paramref name="b"/> * <paramref name="t"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorB Lerp(in ColorB a, in ColorB b, float t) =>
        a + (b - a) * t;

    #endregion
    #region Operators
    /// <summary>
    /// Element-wise equality.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ColorB left, ColorB right) =>
        left.Equals(right);

    /// <summary>
    /// Element-wise inequality.
    /// </summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ColorB left, ColorB right) =>
        !left.Equals(right);

    /// <summary>
    /// Negates the specified color.
    /// </summary>
    /// <param name="color">The color to negate.</param>
    /// <returns>The negated vector.</returns>
    public static ColorB operator -(ColorB color) => Negate(color);

    /// <summary>
    /// Adds two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The summed color.</returns>
    public static ColorB operator +(ColorB left, ColorB right) => Add(left, right);

    /// <summary>
    /// Subtracts the second color from the first.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from subracting <paramref name="right"/> from <paramref name="left"/>.</returns>
    public static ColorB operator -(ColorB left, ColorB right) => Subtract(left, right);

    /// <summary>
    /// Multiplies two colors together.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The product color.</returns>
    public static ColorB operator *(ColorB left, ColorB right) => Multiply(left, right);

    /// <summary>
    /// Divides the first color by the second.
    /// </summary>
    /// <param name="left">The first color.</param>
    /// <param name="right">The second color.</param>
    /// <returns>The color that results from dividing <paramref name="left"/> by <paramref name="right"/>.</returns>
    public static ColorB operator /(ColorB left, ColorB right) => Divide(left, right);

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled color.</returns>
    public static ColorB operator *(ColorB left, float right) => Multiply(left, right);

    /// <summary>
    /// Divides the specified color by a specified scalar value.
    /// </summary>
    /// <param name="left">The color.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    public static ColorB operator /(ColorB left, float right) => Divide(left, right);

    /// <summary>
    /// Multiples the specified color by the specified scalar value.
    /// </summary>
    /// <param name="left">The scalar value.</param>
    /// <param name="right">The color.</param>
    /// <returns>The scaled color.</returns>
    public static ColorB operator *(float left, ColorB right) => Multiply(left, right);
    #endregion
    #region Colors
    /// <summary>
    /// Red (255, 0, 0, 255)
    /// </summary>
    public static readonly ColorB Red = new(255, 0, 0, 255);

    /// <summary>
    /// Transparent (0, 0, 0, 0)
    /// </summary>
    public static readonly ColorB Transparent = new(0, 0, 0, 0);

    /// <summary>
    /// Dark Red (139, 0, 0, 255)
    /// </summary>
    public static readonly ColorB DarkRed = new(139, 0, 0, 255);

    /// <summary>
    /// Green (0, 255, 0, 255)
    /// </summary>
    public static readonly ColorB Green = new(0, 255, 0, 255);

    /// <summary>
    /// Blue (0, 0, 255, 255)
    /// </summary>
    public static readonly ColorB Blue = new(0, 0, 255, 255);

    /// <summary>
    /// Yellow (255, 255, 0, 255)
    /// </summary>
    public static readonly ColorB Yellow = new(255, 255, 0, 255);

    /// <summary>
    /// Grey (128, 128, 128 / 255f, 255)
    /// </summary>
    public static readonly ColorB Grey = new(128, 128, 128, 255);

    /// <summary>
    /// Light Grey (211, 211, 211, 255)
    /// </summary>
    public static readonly ColorB LightGrey = new(211, 211, 211, 255);

    /// <summary>
    /// Cyan (0, 255, 255, 255)
    /// </summary>
    public static readonly ColorB Cyan = new(0, 255, 255, 255);

    /// <summary>
    /// White (255, 255, 255, 255)
    /// </summary>
    public static readonly ColorB White = new(255, 255, 255, 255);

    /// <summary>
    /// Cornflower Blue (100, 149, 237, 255)
    /// </summary>
    public static readonly ColorB CornflowerBlue = new(100, 149, 237, 255);

    /// <summary>
    /// Clear (0, 0, 0, 0)
    /// </summary>
    public static readonly ColorB Clear = new(0, 0, 0, 0);

    /// <summary>
    /// Black (0, 0, 0, 255)
    /// </summary>
    public static readonly ColorB Black = new(0, 0, 0, 255);

    /// <summary>
    /// Pink (255, 192, 203, 255)
    /// </summary>
    public static readonly ColorB Pink = new(255, 192, 203, 255);

    /// <summary>
    /// Orange (255, 165, 0, 255)
    /// </summary>
    public static readonly ColorB Orange = new(255, 165, 0, 255);
    #endregion
}
