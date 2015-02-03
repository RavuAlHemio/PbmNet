using System;
using System.Collections.Generic;
using System.IO;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Implemented by classes which can read streams of bytes and return a specific component value format.
    /// </summary>
    public interface IPixelComponentReader<TPixelComponent>
    {
        /// <summary>
        /// Parses the highest component value from a string of decimal digits.
        /// </summary>
        /// <returns>The highest component value.</returns>
        /// <param name="highestComponentValueString">The highest component string value to parse.</param>
        /// <exception cref="FormatException">Thrown if parsing fails.</exception>
        TPixelComponent ParseHighestComponentValue(string highestComponentValueString);

        /// <summary>
        /// Reads a row from the stream. A row consists of <paramref name="width"/> times
        /// <paramref name="componentCount"/> values, each of a value from <value>0</value> to
        /// <paramref name="highestComponentValue"/>.
        /// </summary>
        /// <param name="stream">The stream to read a row from.</param>
        /// <param name="width">The width of the image being read.</param>
        /// <param name="componentCount">The number of components per pixel.</param>
        /// <param name="highestComponentValue">The highest value a pixel component can assume.</param>
        /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached before enough components
        /// could be read.</exception>
        IEnumerable<TPixelComponent> ReadRow(Stream stream, int width, int componentCount, TPixelComponent highestComponentValue);
    }
}
