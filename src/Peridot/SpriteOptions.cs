namespace Peridot;

/// <summary>
/// Defines sprite options.
/// </summary>
[Flags]
public enum SpriteOptions
{
    /// <summary>
    /// Nothing is applied in sprite rendering.
    /// </summary>
    None,
    /// <summary>
    /// Horizontal flip is applied in sprite rendering.
    /// </summary>
    FlipHorizontally,
    /// <summary>
    /// Vertical flip is applied in sprite rendering.
    /// </summary>
    FlipVertically,
}
