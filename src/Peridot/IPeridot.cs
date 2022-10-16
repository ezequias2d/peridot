using System.Drawing;
namespace Peridot;

/// <summary>
/// Represents a interface for implementation of Peridot.
/// </summary>
public interface IPeridot
{
    /// <summary>
    /// Creates a new image.
    /// </summary>
    /// <param name="description">The description of image.</param>
    /// <returns>A new image.</returns>
    public Image CreateImage(ImageDescription description);

    /// <summary>
    /// Updates a portion of Image with new data.
    /// </summary>
    /// <param name="image">The image to update.</param>
    /// <param name="source">An span with data to upload.</param>
    /// <param name="x">The minimum X value of the updated region.</param>
    /// <param name="y">The minimum Y value of the updated region.</param>
    /// <param name="width">The width of the updated region.</param>
    /// <param name="height">The height of the updated region.</param>
    /// <typeparam name="T">Type of data updated.</typeparam>
    public void UpdateImage<T>(Image image, ReadOnlySpan<T> source, uint x, uint y, uint width, uint height) where T : unmanaged;

    /// <summary>
    /// Copy a part of an image to another.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void CopyImage(Image source, Image destination, uint sx, uint sy, uint dx, uint dy, uint width, uint height);
}

/// <summary>
/// Represents a interface for implementation of Peridot.
/// </summary>
public interface IPeridot<TSpriteBatchDescripton> : IPeridot
{
    /// <summary>
    /// Creates a new sprite batch.
    /// </summary>
    /// <param name="description">The description of sprite batch.</param>
    /// <returns>A new sprite batch.</returns>
    public ISpriteBatch CreateSpriteBatch(TSpriteBatchDescripton description);
}

/// <summary>
/// Represents a interface for implementation of Peridot.
/// </summary>
public interface IPeridot<TImage, TSpriteBatchDescripton> : IPeridot<TSpriteBatchDescripton>
{
    /// <summary>
    /// Creates a new sprite batch.
    /// </summary>
    /// <param name="description">The description of sprite batch.</param>
    /// <returns>A new sprite batch.</returns>
    public new ISpriteBatch<TImage> CreateSpriteBatch(TSpriteBatchDescripton description);
}
