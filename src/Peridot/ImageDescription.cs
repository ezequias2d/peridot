namespace Peridot;

/// <summary>
/// Describes a image in Peridot.
/// </summary>
public struct ImageDescription
{
    /// <summary>
    /// Format of the image data.
    /// </summary>
    /// <value>Internal pixel format of image data.</value>
    public PixelFormat Format { get; set; }

    /// <summary>
    /// The width of image.
    /// </summary>
    /// <value>Image width.</value>
    public uint Width { get; set; }

    /// <summary>
    /// The height of image.
    /// </summary>
    /// <value>Image height.</value>
    public uint Height { get; set; }
}
