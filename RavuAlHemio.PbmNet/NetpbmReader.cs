using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Reads Netpbm images from streams.
    /// </summary>
    public class NetpbmReader
    {
        private enum ValueEncoding
        {
            Plain = 1,
            Binary = 2
        }

        private static readonly ISet<int> NetpbmWhiteSpaceBytes = new HashSet<int>
        {
            ' ',
            '\r',
            '\n',
            '\t',
            '\v',
            '\f'
        };

        private static readonly Encoding UsAsciiEncoding = Encoding.GetEncoding("us-ascii");

        /// <summary>
        /// Skips whitespace (as defined in <see cref="NetpbmWhiteSpaceBytes"/>), if any, and returns the first
        /// non-whitespace byte in the stream. If end-of-file is reached before a non-whitespace character, returns
        /// <value>-1</value>.
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <returns>The first non-whitespace character in <paramref name="stream"/>, or <value>-1</value> if
        /// end-of-file is reached.</returns>
        private static int SkipWhitespaceAndReturnFirstNonWhitespaceByte(Stream stream)
        {
            for (;;)
            {
                int b = stream.ReadByte();
                if (!NetpbmWhiteSpaceBytes.Contains(b))
                {
                    return b;
                }
            }
        }

        /// <summary>
        /// Returns the bytes up to the first whitespace byte (as defined in <see cref="NetpbmWhiteSpaceBytes"/>) in
        /// the stream. Skips Netpbm comments (# until a newline)
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="includeWhitespaceByteInReturnValue">If <c>true</c>, includes the whitespace byte in the return
        /// value. If <c>false</c>, the whitespace byte is discarded.</param>
        /// <param name="throwOnEndOfFile">If <c>true</c>, throws <see cref="EndOfStreamException"/> if end-of-file is
        /// encountered; otherwise, returns the bytes read until then.</param>
        /// <returns>The bytes read from <paramref name="stream"/> until a whitespace byte was encountered.</returns>
        /// <exception cref="EndOfStreamException">Thrown if <paramref name="stream"/> reaches end-of-file before a
        /// whitespace byte is encountered and <paramref name="throwOnEndOfFile"/> is <c>true</c>.</exception>
        private static IEnumerable<byte> ReadUntilFirstWhitespaceByte(Stream stream, bool includeWhitespaceByteInReturnValue,
            bool throwOnEndOfFile)
        {
            for (;;)
            {
                int b = stream.ReadByte();
                if (b == -1)
                {
                    if (throwOnEndOfFile)
                    {
                        throw new EndOfStreamException();
                    }
                    else
                    {
                        yield break;
                    }
                }
                else if (b == '#')
                {
                    // comment
                    while (b != '\r' && b != '\n')
                    {
                        b = stream.ReadByte();
                        if (b == -1)
                        {
                            // comment before EOF
                            if (throwOnEndOfFile)
                            {
                                throw new EndOfStreamException();
                            }
                            else
                            {
                                yield break;
                            }
                        }
                    }
                }
                else if (NetpbmWhiteSpaceBytes.Contains(b))
                {
                    if (includeWhitespaceByteInReturnValue)
                    {
                        yield return (byte) b;
                    }
                    yield break;
                }
                yield return (byte) b;
            }
        }

        /// <summary>
        /// Skips whitespace (as defined in <see cref="NetpbmWhiteSpaceBytes"/>), if any, and returns the bytes up to
        /// the next whitespace byte in the stream (which is discarded).
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="throwOnEndOfFile">If <c>true</c>, throws <see cref="EndOfStreamException"/> if end-of-file is
        /// encountered; otherwise, returns the bytes read until then.</param>
        /// <returns>The bytes read from <paramref name="stream"/> until a whitespace byte after non-whitespace bytes
        /// was encountered.</returns>
        /// <exception cref="EndOfStreamException">Thrown if <paramref name="stream"/> reaches end-of-file before a
        /// whitespace byte is encountered and <paramref name="throwOnEndOfFile"/> is <c>true</c>.</exception>
        private static IEnumerable<byte> SkipWhitespaceAndReadUntilNextWhitespaceByte(Stream stream, bool throwOnEndOfFile)
        {
            // skip whitespace
            int b = SkipWhitespaceAndReturnFirstNonWhitespaceByte(stream);
            if (b == -1)
            {
                if (throwOnEndOfFile)
                {
                    throw new EndOfStreamException();
                }
                else
                {
                    yield break;
                }
            }

            yield return (byte) b;
            foreach (byte b2 in ReadUntilFirstWhitespaceByte(stream, false, throwOnEndOfFile))
            {
                yield return b2;
            }
        }

        /// <summary>
        /// Reads the stream until a newline (0x0A) byte is encountered, and returns all the bytes except the newline.
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="throwOnEndOfFile">If <c>true</c>, throws <see cref="EndOfStreamException"/> if end-of-file is
        /// encountered; otherwise, returns the bytes read until then.</param>
        /// <returns>The bytes read from <paramref name="stream"/> until a newline or end-of-file was
        /// encountered.</returns>
        /// <exception cref="EndOfStreamException">Thrown if <paramref name="stream"/> reaches end-of-file before a
        /// newline byte is encountered and <paramref name="throwOnEndOfFile"/> is <c>true</c>.</exception>
        private static IEnumerable<byte> ReadUntilAndDiscardNewline(Stream stream, bool throwOnEndOfFile)
        {
            for (;;)
            {
                int b = stream.ReadByte();
                if (b == -1)
                {
                    if (throwOnEndOfFile)
                    {
                        throw new EndOfStreamException();
                    }
                    else
                    {
                        yield break;
                    }
                }
                else if (b == '\n')
                {
                    yield break;
                }
                else
                {
                    yield return (byte) b;
                }
            }
        }

        private static bool TryParseIntInvariant(string str, out int result)
        {
            return int.TryParse(str, NumberStyles.None, CultureInfo.InvariantCulture, out result);
        }

        private static string GetUsAsciiString(IEnumerable<byte> bytes)
        {
            var byteArray = bytes.ToArray();
            return UsAsciiEncoding.GetString(byteArray, 0, byteArray.Length);
        }

        private IEnumerable<TPixelFormat> ReadBinaryPBMRow<TPixelFormat>(Stream stream, int pixelCount, IImageFactory<TPixelFormat> imageFactory)
        {
            int byteCount = pixelCount/8;
            if (pixelCount%8 != 0)
            {
                ++byteCount;
            }

            var rowBytes = new byte[byteCount];
            if (!NetpbmUtil.ReadToFillBuffer(stream, rowBytes))
            {
                throw new EndOfStreamException();
            }

            for (int i = 0; i < pixelCount; ++i)
            {
                var byteOffset = i / 8;
                // leftmost bit is the top bit
                var bitShift = 7 - (i % 8);

                if ((rowBytes[byteOffset] & (1 << bitShift)) == 0)
                {
                    yield return imageFactory.ZeroPixelComponentValue;
                }
                else
                {
                    yield return imageFactory.BitmapOnPixelComponentValue;
                }
            }
        }

        private IEnumerable<TPixelFormat> ReadPlainRow<TPixelFormat>(Stream stream, int componentCount,
            IImageFactory<TPixelFormat> imageFactory)
        {
            for (int i = 0; i < componentCount; ++i)
            {
                var valueBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
                var valueString = GetUsAsciiString(valueBytes);
                yield return imageFactory.ParseComponentValue(valueString);
            }
        }

        /// <summary>
        /// Splits the given line into a keyword and a value part.
        /// </summary>
        /// <param name="line">The line to split.</param>
        /// <param name="keyword">The keyword extracted from <paramref name="line"/>.</param>
        /// <param name="value">The value extracted from <paramref name="line"/></param>
        private void SplitKeywordAndValue(string line, out string keyword, out string value)
        {
            for (int whitespaceIndex = 0; whitespaceIndex < line.Length; ++whitespaceIndex)
            {
                if (NetpbmWhiteSpaceBytes.Contains(line[whitespaceIndex]))
                {
                    keyword = line.Substring(0, whitespaceIndex);
                    value = line.Substring(whitespaceIndex + 1);
                    return;
                }
            }
            keyword = line;
            value = null;
        }

        private NetpbmImage<TPixelFormat> ReadPAM<TPixelFormat>(Stream stream, IImageFactory<TPixelFormat> imageFactory)
        {
            // ensure the next character is a newline
            var newline = stream.ReadByte();
            if (newline == -1)
            {
                throw new EndOfStreamException();
            }
            else if (newline != '\n')
            {
                throw new InvalidDataException("byte after magic is not a newline");
            }

            int? width = null;
            int? height = null;
            int? depth = null;
            string maxValueString = null;
            string tupleType = "";
            bool headerEnded = false;

            while (!headerEnded)
            {
                var lineBytes = ReadUntilAndDiscardNewline(stream, true);
                var lineString = GetUsAsciiString(lineBytes);

                if (lineString.Length == 0)
                {
                    // empty line
                    continue;
                }

                if (lineString[0] == '#')
                {
                    // comment
                    continue;
                }

                string keyword, value;
                SplitKeywordAndValue(lineString, out keyword, out value);

                int intValue;
                switch (keyword.ToUpperInvariant())
                {
                    case "WIDTH":
                        if (value == null)
                        {
                            throw new InvalidDataException("PAM header field WIDTH is missing a value");
                        }
                        if (!TryParseIntInvariant(value, out intValue))
                        {
                            throw new InvalidDataException(string.Format("failed to parse width '{0}'", value));
                        }
                        width = intValue;
                        break;
                    case "HEIGHT":
                        if (value == null)
                        {
                            throw new InvalidDataException("PAM header field HEIGHT is missing a value");
                        }
                        if (!TryParseIntInvariant(value, out intValue))
                        {
                            throw new InvalidDataException(string.Format("failed to parse height '{0}'", value));
                        }
                        height = intValue;
                        break;
                    case "DEPTH":
                        if (value == null)
                        {
                            throw new InvalidDataException("PAM header field DEPTH is missing a value");
                        }
                        if (!TryParseIntInvariant(value, out intValue))
                        {
                            throw new InvalidDataException(string.Format("failed to parse depth '{0}'", value));
                        }
                        depth = intValue;
                        break;
                    case "MAXVAL":
                        if (value == null)
                        {
                            throw new InvalidDataException("PAM header field MAXVAL is missing a value");
                        }
                        maxValueString = value;
                        break;
                    case "TUPLTYPE":
                        if (value == null)
                        {
                            // never mind
                        }
                        else if (tupleType.Length == 0)
                        {
                            tupleType = value;
                        }
                        else
                        {
                            tupleType += " " + value;
                        }
                        break;
                    case "ENDHDR":
                        headerEnded = true;
                        break;
                }
            }

            if (!width.HasValue)
            {
                throw new InvalidDataException("PAM header missing width");
            }
            if (!height.HasValue)
            {
                throw new InvalidDataException("PAM header missing height");
            }
            if (!depth.HasValue)
            {
                throw new InvalidDataException("PAM header missing depth");
            }
            if (maxValueString == null)
            {
                throw new InvalidDataException("PAM header missing maximum value");
            }
            // don't worry if the tuple type is missing

            var maxValue = imageFactory.ParseHighestComponentValue(maxValueString);

            Component[] components;
            if (depth.Value == 1 && (tupleType == "BLACKANDWHITE" || tupleType == "GRAYSCALE"))
            {
                // note: doesn't check if a BLACKANDWHITE image is really only two-valued
                components = new[] {Component.White};
            }
            else if (depth.Value == 2 && (tupleType == "BLACKANDWHITE_ALPHA" || tupleType == "GRAYSCALE_ALPHA"))
            {
                // note: doesn't check if a BLACKANDWHITE image is really only two-valued
                components = new[] { Component.White, Component.Alpha };
            }
            else if (depth.Value == 3 && tupleType == "RGB")
            {
                components = new[] {Component.Red, Component.Green, Component.Blue};
            }
            else if (depth.Value == 4)
            {
                if (tupleType == "RGB_ALPHA")
                {
                    components = new[] {Component.Red, Component.Green, Component.Blue, Component.Alpha};
                }
                else if (tupleType == "CMYK")
                {
                    components = new[] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black};
                }
            }

            // final newline has been discarded by ReadUntilAndDiscardNewline

            // TODO
            throw new NotImplementedException();
        }

        private NetpbmImage<TPixelFormat> ReadPBM<TPixelFormat>(Stream stream, ValueEncoding valueEncoding, IImageFactory<TPixelFormat> imageFactory)
        {
            int width;
            var widthBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var widthString = GetUsAsciiString(widthBytes);
            if (!TryParseIntInvariant(widthString, out width))
            {
                throw new InvalidDataException(string.Format("failed to parse width '{0}'", widthString));
            }

            int height;
            var heightBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var heightString = GetUsAsciiString(heightBytes);
            if (!TryParseIntInvariant(heightString, out height))
            {
                throw new InvalidDataException(string.Format("failed to parse height '{0}'", heightString));
            }

            // final byte of whitespace has been discarded by SkipWhitespaceAndReadUntilNextWhitespaceByte

            // read the bits!
            var rows = new List<IEnumerable<TPixelFormat>>();
            if (valueEncoding == ValueEncoding.Binary)
            {
                for (int r = 0; r < height; ++r)
                {
                    rows.Add(ReadBinaryPBMRow(stream, width, imageFactory));
                }
            }
            else if (valueEncoding == ValueEncoding.Plain)
            {
                for (int r = 0; r < height; ++r)
                {
                    rows.Add(ReadPlainRow(stream, width, imageFactory));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("valueEncoding", valueEncoding, "unknown value encoding");
            }

            return imageFactory.MakeImage(width, height, imageFactory.BitmapOnPixelComponentValue,
                new[] {Component.Black}, rows);
        }

        private NetpbmImage<TPixelFormat> ReadPGMOrPPM<TPixelFormat>(Stream stream, ValueEncoding valueEncoding, IImageFactory<TPixelFormat> imageFactory, params Component[] components)
        {
            int width;
            var widthBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var widthString = GetUsAsciiString(widthBytes);
            if (!TryParseIntInvariant(widthString, out width))
            {
                throw new InvalidDataException(string.Format("failed to parse width '{0}'", widthString));
            }

            int height;
            var heightBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var heightString = GetUsAsciiString(heightBytes);
            if (!TryParseIntInvariant(heightString, out height))
            {
                throw new InvalidDataException(string.Format("failed to parse height '{0}'", heightString));
            }

            var highestValueBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var highestValueString = GetUsAsciiString(heightBytes);
            TPixelFormat highestValue = imageFactory.ParseHighestComponentValue(highestValueString);

            // final byte of whitespace has been discarded by SkipWhitespaceAndReadUntilNextWhitespaceByte

            // read the bits!
            var rows = new List<IEnumerable<TPixelFormat>>();
            if (valueEncoding == ValueEncoding.Binary)
            {
                for (int r = 0; r < height; ++r)
                {
                    rows.Add(imageFactory.ReadRow(stream, width, components.Length, highestValue));
                }
            }
            else if (valueEncoding == ValueEncoding.Plain)
            {
                for (int r = 0; r < height; ++r)
                {
                    rows.Add(ReadPlainRow(stream, width * components.Length, imageFactory));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("valueEncoding", valueEncoding, "unknown value encoding");
            }

            return imageFactory.MakeImage(width, height, highestValue, components, rows);
        }

        public NetpbmImage<TPixelFormat> ReadImage<TPixelFormat>(Stream stream, IImageFactory<TPixelFormat> imageFactory)
        {
            // P?
            var magic = new byte[2];
            if (stream.Read(magic, 0, 2) < 2)
            {
                throw new EndOfStreamException("reached end of stream while reading magic value");
            }

            if (magic[0] != 'P')
            {
                throw new InvalidDataException("magic value does not start with 'P'");
            }

            // choose format
            if (magic[1] == '1')
            {
                return ReadPBM(stream, ValueEncoding.Plain, imageFactory);
            }
            else if (magic[1] == '2')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Plain, imageFactory, Component.White);
            }
            else if (magic[1] == '3')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Plain, imageFactory, Component.Red, Component.Green, Component.Blue);
            }
            else if (magic[1] == '4')
            {
                return ReadPBM<TPixelFormat>(stream, ValueEncoding.Binary, imageFactory);
            }
            else if (magic[1] == '5')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Binary, imageFactory, Component.White);
            }
            else if (magic[1] == '6')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Binary, imageFactory, Component.Red, Component.Green, Component.Blue);
            }
            else if (magic[1] == '7')
            {
                return ReadPAM<TPixelFormat>(stream, imageFactory);
            }
            else
            {
                throw new InvalidDataException("magic value discriminator isn't between 1 and 7");
            }
        }
    }
}
