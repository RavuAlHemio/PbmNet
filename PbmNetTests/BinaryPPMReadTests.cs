using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class BinaryPPMReadTests
    {
        [Fact]
        public void ValidTwoTimesTwo()
        {
            var redPixel = new byte[] {0x01, 0x00, 0x00};
            var blackPixel = new byte[] {0x00, 0x00, 0x00};

            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '2', ' ', '2', ' ', '1', '\n',
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(2, image.Width);
            Assert.Equal(2, image.Height);
            Assert.Equal(1, image.HighestComponentValue);
            Assert.Equal(3, image.Components.Count);
            Assert.Equal(Component.Red, image.Components[0]);
            Assert.Equal(Component.Green, image.Components[1]);
            Assert.Equal(Component.Blue, image.Components[2]);
            Assert.Equal((IEnumerable<byte>)blackPixel, (IEnumerable<byte>)image.GetNativePixel(0, 0));
            Assert.Equal((IEnumerable<byte>)redPixel, (IEnumerable<byte>)image.GetNativePixel(1, 0));
            Assert.Equal((IEnumerable<byte>)redPixel, (IEnumerable<byte>)image.GetNativePixel(0, 1));
            Assert.Equal((IEnumerable<byte>)blackPixel, (IEnumerable<byte>)image.GetNativePixel(1, 1));
        }

        [Fact]
        public void ValidThreeTimesThreeLotsOfWhitespace()
        {
            var redRow = new byte[] { 1, 0, 0 };
            var greenRow = new byte[] { 0, 1, 0 };
            var blueRow = new byte[] { 0, 0, 1 };
            var bodyRawBytes = new object[]
            {
                'P', '6',
                ' ', '\n', ' ', ' ', ' ',
                '3', ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', ' ', '1', ' ',
                0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x01
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(3, image.Width);
            Assert.Equal(3, image.Height);
            Assert.Equal(1, image.HighestComponentValue);
            Assert.Equal(3, image.Components.Count);
            Assert.Equal(Component.Red, image.Components[0]);
            Assert.Equal(Component.Green, image.Components[1]);
            Assert.Equal(Component.Blue, image.Components[2]);
            Assert.Equal((IEnumerable<byte>)redRow, (IEnumerable<byte>)image.GetNativePixel(0, 0));
            Assert.Equal((IEnumerable<byte>)redRow, (IEnumerable<byte>)image.GetNativePixel(1, 0));
            Assert.Equal((IEnumerable<byte>)redRow, (IEnumerable<byte>)image.GetNativePixel(2, 0));
            Assert.Equal((IEnumerable<byte>)greenRow, (IEnumerable<byte>)image.GetNativePixel(0, 1));
            Assert.Equal((IEnumerable<byte>)greenRow, (IEnumerable<byte>)image.GetNativePixel(1, 1));
            Assert.Equal((IEnumerable<byte>)greenRow, (IEnumerable<byte>)image.GetNativePixel(2, 1));
            Assert.Equal((IEnumerable<byte>)blueRow, (IEnumerable<byte>)image.GetNativePixel(0, 2));
            Assert.Equal((IEnumerable<byte>)blueRow, (IEnumerable<byte>)image.GetNativePixel(1, 2));
            Assert.Equal((IEnumerable<byte>)blueRow, (IEnumerable<byte>)image.GetNativePixel(2, 2));
        }

        [Fact]
        public void ValidSixTimesSixGradient()
        {
            var referenceRows = new[]
            {
                new byte[] {255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0,   0},
                new byte[] {  0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0},
                new byte[] {  0,   0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0},
                new byte[] {255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0,   0},
                new byte[] {255,   0, 255, 204,   0, 204, 153,   0, 153, 102,   0, 102,  51,   0,  51,   0,   0,   0},
                new byte[] {  0, 255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0}
            };
            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '6', ' ', '6', ' ', '2', '5', '5', '\n',
                255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0,   0,
                  0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0,
                  0,   0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,
                255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0,   0,
                255,   0, 255, 204,   0, 204, 153,   0, 153, 102,   0, 102,  51,   0,  51,   0,   0,   0,
                  0, 255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(6, image.Width);
            Assert.Equal(6, image.Height);
            Assert.Equal(255, image.HighestComponentValue);
            Assert.Equal(3, image.Components.Count);
            Assert.Equal(Component.Red, image.Components[0]);
            Assert.Equal(Component.Green, image.Components[1]);
            Assert.Equal(Component.Blue, image.Components[2]);
            Assert.Equal((IEnumerable<byte>)referenceRows[0], (IEnumerable<byte>)image.NativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceRows[1], (IEnumerable<byte>)image.NativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceRows[2], (IEnumerable<byte>)image.NativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceRows[3], (IEnumerable<byte>)image.NativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceRows[4], (IEnumerable<byte>)image.NativeRows[4]);
            Assert.Equal((IEnumerable<byte>)referenceRows[5], (IEnumerable<byte>)image.NativeRows[5]);
        }

        [Fact]
        public void ValidOneTimesOne16Bit()
        {
            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '1', ' ', '1', ' ', '6', '5', '5', '3', '5', '\n',
                0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image16Factory();
            var reader = new NetpbmReader();
            NetpbmImage16 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage16)reader.ReadImage(bodyStream, factory);
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(1, image.Width);
            Assert.Equal(1, image.Height);
            Assert.Equal(65535, image.HighestComponentValue);
            Assert.Equal(3, image.Components.Count);
            Assert.Equal(Component.Red, image.Components[0]);
            Assert.Equal(Component.Green, image.Components[1]);
            Assert.Equal(Component.Blue, image.Components[2]);
            Assert.Equal(0x1234, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(0x5678, image.GetNativePixel(0, 0)[1]);
            Assert.Equal(0x9ABC, image.GetNativePixel(0, 0)[2]);
        }

        [Fact]
        public void TwoTimesTwoTooFewPixels()
        {
            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '2', ' ', '2', ' ', '1', '\n',
                0x00, 0x00, 0x01
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<EndOfStreamException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    reader.ReadImage(bodyStream, factory);
                }
            });
        }

        [Fact]
        public void TwoTimesTwoValueOutOfRange()
        {
            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '2', ' ', '2', ' ', '1', '\n',
                0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    reader.ReadImage(bodyStream, factory);
                }
            });
        }

        [Fact]
        public void SixTimesSixGradientValueOutOfRange()
        {
            var bodyRawBytes = new object[]
            {
                'P', '6',
                '\n',
                '6', ' ', '6', ' ', '2', '5', '4', '\n',
                255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0,   0,
                  0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,   0,
                  0,   0, 255,   0,   0, 204,   0,   0, 153,   0,   0, 102,   0,   0,  51,   0,   0,   0,
                255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0,   0,
                255,   0, 255, 204,   0, 204, 153,   0, 153, 102,   0, 102,  51,   0,  51,   0,   0,   0,
                  0, 255, 255,   0, 204, 204,   0, 153, 153,   0, 102, 102,   0,  51,  51,   0,   0,   0
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    reader.ReadImage(bodyStream, factory);
                }
            });
        }
    }
}
