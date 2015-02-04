using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace RavuAlHemio.PbmNet
{
    public static class ImageFactories
    {
        public class Image8Factory : IImageFactory<byte>
        {
            public byte ParseComponentValue(string componentValueString)
            {
                return byte.Parse(componentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public byte ParseHighestComponentValue(string highestComponentValueString)
            {
                return ParseComponentValue(highestComponentValueString);
            }

            public byte BitmapOnPixelComponentValue
            {
                get { return 1; }
            }

            public byte ZeroPixelComponentValue
            {
                get { return 0; }
            }

            public IEnumerable<byte> ReadRow(Stream stream, int width, int componentCount, byte highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new byte[readCount];
                if (!NetpbmUtil.ReadToFillBuffer(stream, ret))
                {
                    throw new EndOfStreamException();
                }
                return ret;
            }

            public NetpbmImage<byte> MakeImage(int width, int height, byte highestComponentValue, IEnumerable<Component> components, IEnumerable<IEnumerable<byte>> pixelData)
            {
                return new NetpbmImage8(width, height, highestComponentValue, components, pixelData);
            }
        }

        public class Image16Factory : IImageFactory<ushort>
        {
            public ushort ParseComponentValue(string componentValueString)
            {
                return ushort.Parse(componentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public ushort ParseHighestComponentValue(string highestComponentValueString)
            {
                return ParseComponentValue(highestComponentValueString);
            }

            public ushort BitmapOnPixelComponentValue
            {
                get { return 1; }
            }

            public ushort ZeroPixelComponentValue
            {
                get { return 0; }
            }

            public IEnumerable<ushort> ReadRow(Stream stream, int width, int componentCount, ushort highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new List<ushort>();
                var buf = new byte[2];
                for (int i = 0; i < readCount; ++i)
                {
                    if (!NetpbmUtil.ReadToFillBuffer(stream, buf))
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

            public NetpbmImage<ushort> MakeImage(int width, int height, ushort highestComponentValue, IEnumerable<Component> components, IEnumerable<IEnumerable<ushort>> pixelData)
            {
                return new NetpbmImage16(width, height, highestComponentValue, components, pixelData);
            }
        }

        public class Image32Factory : IImageFactory<uint>
        {
            public uint ParseComponentValue(string componentValueString)
            {
                return uint.Parse(componentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public uint ParseHighestComponentValue(string highestComponentValueString)
            {
                return ParseComponentValue(highestComponentValueString);
            }

            public uint BitmapOnPixelComponentValue
            {
                get { return 1; }
            }

            public uint ZeroPixelComponentValue
            {
                get { return 0; }
            }

            public IEnumerable<uint> ReadRow(Stream stream, int width, int componentCount, uint highestComponentValue)
            {
                var readCount = width * componentCount;
                var ret = new List<uint>();
                var buf = new byte[4];
                for (int i = 0; i < readCount; ++i)
                {
                    if (!NetpbmUtil.ReadToFillBuffer(stream, buf))
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

            public NetpbmImage<uint> MakeImage(int width, int height, uint highestComponentValue, IEnumerable<Component> components, IEnumerable<IEnumerable<uint>> pixelData)
            {
                return new NetpbmImage32(width, height, highestComponentValue, components, pixelData);
            }
        }

        public class ImageBigIntegerFactory : IImageFactory<BigInteger>
        {
            public BigInteger ParseComponentValue(string componentValueString)
            {
                return BigInteger.Parse(componentValueString, NumberStyles.None, CultureInfo.InvariantCulture);
            }

            public BigInteger ParseHighestComponentValue(string highestComponentValueString)
            {
                return ParseComponentValue(highestComponentValueString);
            }

            public BigInteger BitmapOnPixelComponentValue
            {
                get { return BigInteger.One; }
            }

            public BigInteger ZeroPixelComponentValue
            {
                get { return BigInteger.Zero; }
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
                    if (!NetpbmUtil.ReadToFillBuffer(stream, buf))
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

            public NetpbmImage<BigInteger> MakeImage(int width, int height, BigInteger highestComponentValue, IEnumerable<Component> components,
                IEnumerable<IEnumerable<BigInteger>> pixelData)
            {
                return new NetpbmImageBigInteger(width, height, highestComponentValue, components, pixelData);
            }
        }
    }
}
