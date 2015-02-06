using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class BinaryPGMReadTests
    {
        [Fact]
        public void ValidTwoTimesTwo()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
                '\n',
                '2', ' ', '2', ' ', '1', '\n',
                0x00, 0x01,
                0x01, 0x00
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
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidTwoTimesTwoWithComment()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
                '\n',
                '2', ' ', '2', ' ', '#', 'l', 'o', 'l', '\n', '1', '\n',
                0x00, 0x01,
                0x01, 0x00
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
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidTwoTimesTwoWithCommentGauntlet()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
                '\n',
                '0', '#', 'o', 'm', 'g', '\n', '2', ' ', '2', ' ', '#', 'l', 'o', 'l', '\n', '#', ' ', 'r', 'o', 'f', 'l', '\n', '1', '\n',
                0x00, 0x01,
                0x01, 0x00
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
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidThreeTimesTwoLotsOfWhitespace()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
                ' ', '\n', ' ', ' ', ' ',
                '3', ' ', ' ', ' ', ' ', ' ', ' ', '2', ' ', ' ', '1', ' ',
                0x00, 0x01, 0x01,
                0x01, 0x00, 0x00
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
            Assert.Equal(2, image.Height);
            Assert.Equal(1, image.HighestComponentValue);
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(2, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(2, 1)[0]);
        }

        [Fact]
        public void ValidOneTimesOne16Bit()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
                '\n',
                '1', ' ', '1', ' ', '6', '5', '5', '3', '5', '\n',
                0x12, 0x34
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
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal(0x1234, image.GetNativePixel(0, 0)[0]);
        }

        [Fact]
        public void ValidSixTimesSixGradient()
        {
            var referenceRow = new byte[] { 255, 204, 153, 102, 51, 0 };
            var bodyRawBytes = new object[]
            {
                'P', '5',
                '\n',
                '6', ' ', '6', ' ', '2', '5', '5', '\n',
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0
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
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.White, image.Components[0]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[4]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.NativeRows[5]);
        }

        [Fact]
        public void TwoTimesTwoTooFewPixels()
        {
            var bodyRawBytes = new object[]
            {
                'P', '5',
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
                'P', '5',
                '\n',
                '2', ' ', '2', ' ', '1', '\n',
                0x00, 0x02, 0x01, 0x00
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
                'P', '5',
                '\n',
                '6', ' ', '6', ' ', '2', '5', '4', '\n',
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0,
                255, 204, 153, 102, 51, 0
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
