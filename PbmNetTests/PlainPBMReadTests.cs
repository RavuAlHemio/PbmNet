using System;
using System.IO;
using System.Text;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class PlainPBMReadTests
    {
        private static readonly Encoding UsAsciiEncoding = Encoding.GetEncoding("us-ascii",
            new EncoderExceptionFallback(), new DecoderExceptionFallback());

        [Fact]
        public void ValidTwoTimesTwo()
        {
            const string bodyString = "P1\n2 2\n0 1 1 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
            }

            Assert.Equal(2, image.Width);
            Assert.Equal(2, image.Height);
            Assert.Equal(1, image.HighestComponentValue);
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.Black, image.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidThreeTimesTwoLotsOfWhitespace()
        {
            const string bodyString = "P1 \n   3      2 \n 0   1  1  \t  1    0  0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
            }

            Assert.Equal(3, image.Width);
            Assert.Equal(2, image.Height);
            Assert.Equal(1, image.HighestComponentValue);
            Assert.Equal(1, image.Components.Count);
            Assert.Equal(Component.Black, image.Components[0]);
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
            const string bodyString = "P1\n2 2\n0 0 1";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
            const string bodyString = "P1\n2 2\n0 2 1 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
        public void TwoTimesTwoValueVeryOutOfRange()
        {
            const string bodyString = "P1\n2 2\n0 1048576 1 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
        public void TwoTimesTwoValueBlatantlyOutOfRange()
        {
            const string bodyString = "P1\n2 2\n0 36893488147419103232 1 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
