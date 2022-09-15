namespace Peridot;

/// <summary>
/// Specifies the pixel format.
/// </summary>
public enum PixelFormat
{
    Undefined,
    /// <summary>
    /// One-channel, byte storage.
    /// </summary>
    R8,
    /// <summary>
    /// One-channel, short storage.
    /// </summary>
    R16,
    /// <summary>
    /// Four channels, byte storage.
    /// </summary>
    BGRA8,
    /// <summary>
    /// Four channels, float storage.
    /// </summary>
    RGBAF32,
    /// <summary>
    /// Two-channels, byte storage.
    /// </summary>
    RG8,
    /// <summary>
    /// Two-channels, short storage.
    /// </summary>
    RG16,
    /// <summary>
    /// Four channels, signed integer storage.
    /// </summary>
    RGBAI32,
    /// <summary>
    /// Four channels, byte storage.
    /// </summary>
    RGBA8,
}
