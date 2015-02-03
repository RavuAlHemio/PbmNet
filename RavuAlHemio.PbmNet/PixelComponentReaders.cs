using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace RavuAlHemio.PbmNet
{
    public static class PixelComponentReaders
    {
        /// <summary>
        /// Reads from a stream until end-of-file or the buffer is filled, and returns whether the buffer was filled.
        /// </summary>
        /// <returns><c>true</c> if the buffer was filled; <c>false</c> if end-of-file was encountered.</returns>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="buffer">The buffer to fill.</param>
        private static bool ReadToFillBuffer(Stream stream, byte[] buffer)
        {
            int offset = 0;
            for (;;)
            {
                int remainingBytes = buffer.Length - offset;
                if (remainingBytes == 0)
                {
                    return true;
                }
                var readBytes = stream.Read(buffer, offset, remainingBytes);
                if (readBytes == 0)
                {
                    // EOF
                    return false;
                }
                offset += readBytes;
            }
        }

        public class UInt8Reader : IPixelComponentReader<byte>
        {
            public byte ParseHighestComponentValue(string highestComponentValueString)
            {
                return byte.Parse(highestComponentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public IEnumerable<byte> ReadRow(Stream stream, int width, int componentCount, byte highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new byte[readCount];
                if (!ReadToFillBuffer(stream, ret))
                {
                    throw new EndOfStreamException();
                }
                return ret;
            }
        }

        public class UInt16Reader : IPixelComponentReader<ushort>
        {
            public ushort ParseHighestComponentValue(string highestComponentValueString)
            {
                return ushort.Parse(highestComponentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public IEnumerable<ushort> ReadRow(Stream stream, int width, int componentCount, ushort highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new List<ushort>();
                var buf = new byte[2];
                for (int i = 0; i < readCount; ++i)
                {
                    if (!ReadToFillBuffer(stream, buf))
                    {
                        throw new EndOfStreamException();
                    }

                    // big-endian
                    ushort val = (ushort)(
                        ((uint)buf[0] << 8) |
                        ((uint)buf[1] << 0)
                    );
                    ret.Add(val);
                }
                return ret;
            }
        }

        public class UInt32Reader : IPixelComponentReader<uint>
        {
            public uint ParseHighestComponentValue(string highestComponentValueString)
            {
                return uint.Parse(highestComponentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public IEnumerable<uint> ReadRow(Stream stream, int width, int componentCount, uint highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new List<uint>();
                var buf = new byte[4];
                for (int i = 0; i < readCount; ++i)
                {
                    if (!ReadToFillBuffer(stream, buf))
                    {
                        throw new EndOfStreamException();
                    }

                    // big-endian
                    uint val =
                        ((uint)buf[0] << 24) |
                        ((uint)buf[1] << 16) |
                        ((uint)buf[2] <<  8) |
                        ((uint)buf[3] <<  0)
                    ;
                    ret.Add(val);
                }
                return ret;
            }
        }

        public class BigIntegerReader : IPixelComponentReader<BigInteger>
        {
            public BigInteger ParseHighestComponentValue(string highestComponentValueString)
            {
                return BigInteger.Parse(highestComponentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public IEnumerable<BigInteger> ReadRow(Stream stream, int width, int componentCount, BigInteger highestComponentValue)
            {
                // find how many bytes each value needs
                var highestBytes = highestComponentValue.ToByteArray();
                var bytesPerValue = highestBytes.Length;
                if (highestBytes[highestBytes.Length - 1] == 0)
                {
                    // additional byte to ensure number is positive
                    // the file's encoding doesn't use this
                    --bytesPerValue;
                }

                var readCount = width * componentCount;
                var ret = new List<BigInteger>();
                var buf = new byte[bytesPerValue];
                for (int i = 0; i < readCount; ++i)
                {
                    if (!ReadToFillBuffer(stream, buf))
                    {
                        throw new EndOfStreamException();
                    }

                    // values are big endian, BigInteger wants little
                    Array.Reverse(buf);

                    // is the potential sign bit set?
                    if ((buf[buf.Length - 1] & 0x80) != 0)
                    {
                        // add a zero byte at the end to make sure the number is positive
                        var newBuf = new byte[buf.Length + 1];
                        Array.Copy(buf, 0, newBuf, 0, buf.Length);
                        newBuf[buf.Length] = 0;
                        ret.Add(new BigInteger(newBuf));
                    }
                    else
                    {
                        ret.Add(new BigInteger(buf));
                    }
                }
                return ret;
            }
        }
    }
}
