using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A NetPBM-compatible image.
    /// </summary>
    public interface INetPbmImage
    {
        /// <summary>
        /// The width of the image, in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the image, in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The number of bytes used to represent a single component of a single pixel.
        /// </summary>
        int BytesPerPixelComponent { get; }

        /// <summary>
        /// The list of color components that make up this picture.
        /// </summary>
        IList<Component> Components { get; }

        /// <summary>
        /// Obtains the scaled values of the pixel at the given coordinates, component by component (in the order given
        /// by <see cref="Components"/>). Each value lies within the interval [<value>0.0</value>, <value>1.0</value>].
        /// </summary>
        /// <param name="x">The X coordinate of the pixel to fetch.</param>
        /// <param name="y">The Y coordinate of the pixel to fetch.</param>
        /// <returns>A list of pixel values scaled to the interval [<value>0.0</value>, <value>1.0</value>].</returns>
        IList<double> GetScaledPixel(int x, int y);
    }
}
