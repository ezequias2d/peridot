namespace Peridot;

/// <summary>
/// Defines sort mode of a <see cref="ISpriteBatch"/>.
/// </summary>
public enum SortMode
{
    /// <summary>
    /// Sprites are draw in call sequence.
    /// </summary>
    None,
    /// <summary>
    /// Sprites are sorted by depth in back-to-front order before drawing.
    /// </summary>
    BackToFront,
    /// <summary>
    /// Sprites are sorted by depth in front-to-back order before drawing.
    /// </summary>
    FrontToBack
}
