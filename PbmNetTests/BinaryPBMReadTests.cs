using System;
using System.IO;
using System.Linq;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class BinaryPBMReadTests
    {
        [Fact]
        public void ValidTwoTimesTwo()
        {
            var bodyRawBytes = new object[]
            {
                'P', '4',
                '\n',
                '2', ' ', '2', '\n',
                0x40,
                0x80
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(2, image.Header.Width);
            Assert.Equal(2, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(1, image.Header.Components.Count);
            Assert.Equal(Component.Black, image.Header.Components[0]);
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
                'P', '4',
                ' ', '\n', ' ', ' ', ' ',
                '3', ' ', ' ', ' ', ' ', ' ', ' ', '2', ' ',
                0x60,
                0x80
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(3, image.Header.Width);
            Assert.Equal(2, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(1, image.Header.Components.Count);
            Assert.Equal(Component.Black, image.Header.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(2, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(2, 1)[0]);
        }

        [Fact]
        public void TwoTimesTwoTooFewPixels()
        {
            var bodyRawBytes = new object[]
            {
                'P', '4',
                '9', ' ', '1', ' ',
                0x01
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<EndOfStreamException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }

        [Fact]
        public void TwoTimesTwoValueOutOfRange()
        {
            var bodyRawBytes = new object[]
            {
                'P', '4',
                '9', ' ', '1', ' ',
                0x01
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<EndOfStreamException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }

        [Fact]
        public void TwoImagesTwoTimesTwo()
        {
            var bodyRawBytes = new object[]
            {
                'P', '4',
                '\n',
                '2', ' ', '2', '\n',
                0x40,
                0x80,
                'P', '4',
                '\n',
                '2', ' ', '2', '\n',
                0xC0,
                0x00
            };
            var bodyBytes = bodyRawBytes.Select(Convert.ToByte).ToArray();

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 firstImage;
            NetpbmImage8 secondImage;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                firstImage = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                firstImage.LoadData();
                secondImage = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                secondImage.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(2, firstImage.Header.Width);
            Assert.Equal(2, firstImage.Header.Height);
            Assert.Equal(1, firstImage.Header.HighestComponentValue);
            Assert.Equal(1, firstImage.Header.Components.Count);
            Assert.Equal(Component.Black, firstImage.Header.Components[0]);
            Assert.Equal(0, firstImage.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, firstImage.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, firstImage.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, firstImage.GetNativePixel(1, 1)[0]);

            Assert.Equal(2, secondImage.Header.Width);
            Assert.Equal(2, secondImage.Header.Height);
            Assert.Equal(1, secondImage.Header.HighestComponentValue);
            Assert.Equal(1, secondImage.Header.Components.Count);
            Assert.Equal(Component.Black, secondImage.Header.Components[0]);
            Assert.Equal(1, secondImage.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, secondImage.GetNativePixel(1, 0)[0]);
            Assert.Equal(0, secondImage.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, secondImage.GetNativePixel(1, 1)[0]);
        }
    }
}
