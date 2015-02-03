using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
            ' ', '\r', '\n', '\t', '\v', '\f'
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
        private static IList<byte> ReadUntilFirstWhitespaceByte(Stream stream, bool includeWhitespaceByteInReturnValue,
            bool throwOnEndOfFile)
        {
            var ret = new List<byte>();
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
                        return ret;
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
                                return ret;
                            }
                        }
                    }
                }
                else if (NetpbmWhiteSpaceBytes.Contains(b))
                {
                    if (includeWhitespaceByteInReturnValue)
                    {
                        ret.Add((byte) b);
                    }
                    return ret;
                }
                ret.Add((byte) b);
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
        private static IList<byte> SkipWhitespaceAndReadUntilNextWhitespaceByte(Stream stream, bool throwOnEndOfFile)
        {
            var ret = new List<byte>();
            
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
                    return ret;
                }
            }

            ret.Add((byte) b);
            ret.AddRange(ReadUntilFirstWhitespaceByte(stream, false, throwOnEndOfFile));
            return ret;
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

        private NetpbmImage<TPixelFormat> ReadPAM<TPixelFormat>(Stream stream)
        {
            // TODO
            throw new NotImplementedException();
        }

        private NetpbmImage<TPixelFormat> ReadPBM<TPixelFormat>(Stream stream, ValueEncoding valueEncoding)
        {
            int width;
            var widthBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var widthString = GetUsAsciiString(widthBytes);
            if (!TryParseIntInvariant(widthString, out width))
            {
                throw new FormatException(string.Format("failed to parse width '{0}'", widthString));
            }

            int height;
            var heightBytes = SkipWhitespaceAndReadUntilNextWhitespaceByte(stream, true);
            var heightString = GetUsAsciiString(heightBytes);
            if (!TryParseIntInvariant(heightString, out height))
            {
                throw new FormatException(string.Format("failed to parse height '{0}'", heightString));
            }

            // final byte of whitespace has been discarded by SkipWhitespaceAndReadUntilNextWhitespaceByte

            // TODO
            throw new NotImplementedException();
        }

        private NetpbmImage<TPixelFormat> ReadPGMOrPPM<TPixelFormat>(Stream stream, ValueEncoding valueEncoding, params Component[] components)
        {
            // TODO
            throw new NotImplementedException();
        }

        public NetpbmImage<TPixelFormat> ReadImage<TPixelFormat>(Stream stream)
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
                return ReadPBM<TPixelFormat>(stream, ValueEncoding.Plain);
            }
            else if (magic[1] == '2')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Plain, Component.White);
            }
            else if (magic[1] == '3')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Plain, Component.Red, Component.Green, Component.Blue);
            }
            else if (magic[1] == '4')
            {
                return ReadPBM<TPixelFormat>(stream, ValueEncoding.Binary);
            }
            else if (magic[1] == '5')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Binary, Component.White);
            }
            else if (magic[1] == '6')
            {
                return ReadPGMOrPPM<TPixelFormat>(stream, ValueEncoding.Binary, Component.Red, Component.Green, Component.Blue);
            }
            else if (magic[1] == '7')
            {
                return ReadPAM<TPixelFormat>(stream);
            }
            else
            {
                throw new InvalidDataException("magic value discriminator isn't between 1 and 7");
            }
        }
    }
}
