using System.Collections.Generic;
using System.IO;
using System.Text;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class PlainPPMReadTests
    {
        private static readonly Encoding UsAsciiEncoding = Encoding.GetEncoding("us-ascii",
            new EncoderExceptionFallback(), new DecoderExceptionFallback());

        [Fact]
        public void ValidThreeTimesThree()
        {
            var redRow = new byte[] {1, 0, 0};
            var greenRow = new byte[] {0, 1, 0};
            var blueRow = new byte[] {0, 0, 1};
            const string bodyString = "P3\n3 3 1\n1 0 0 1 0 0 1 0 0 0 1 0 0 1 0 0 1 0 0 0 1 0 0 1 0 0 1";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
            Assert.Equal(3, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(3, image.Header.Components.Count);
            Assert.Equal(Component.Red, image.Header.Components[0]);
            Assert.Equal(Component.Green, image.Header.Components[1]);
            Assert.Equal(Component.Blue, image.Header.Components[2]);
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
        public void ValidThreeTimesThreeLotsOfWhitespace()
        {
            var redRow = new byte[] { 1, 0, 0 };
            var greenRow = new byte[] { 0, 1, 0 };
            var blueRow = new byte[] { 0, 0, 1 };
            const string bodyString = "P3 \n   3      3  1 \n 1  0      0   1  0      0   1    0       0  0        1  0     0      1   0     0  1     0  0  0    1  0  0     1  0  0       1";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
            Assert.Equal(3, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(3, image.Header.Components.Count);
            Assert.Equal(Component.Red, image.Header.Components[0]);
            Assert.Equal(Component.Green, image.Header.Components[1]);
            Assert.Equal(Component.Blue, image.Header.Components[2]);
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
            const string bodyString = "P3\n6 6 255\n255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0 0 0\n0 255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0 0\n0 0 255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0\n255 255 0 204 204 0 153 153 0 102 102 0 51 51 0 0 0 0\n255 0 255 204 0 204 153 0 153 102 0 102 51 0 51 0 0 0\n0 255 255 0 204 204 0 153 153 0 102 102 0 51 51 0 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);
             
            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            NetpbmImage8 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage8)reader.ReadImage(bodyStream, factory);
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(6, image.Header.Width);
            Assert.Equal(6, image.Header.Height);
            Assert.Equal(255, image.Header.HighestComponentValue);
            Assert.Equal(3, image.Header.Components.Count);
            Assert.Equal(Component.Red, image.Header.Components[0]);
            Assert.Equal(Component.Green, image.Header.Components[1]);
            Assert.Equal(Component.Blue, image.Header.Components[2]);
            Assert.Equal((IEnumerable<byte>)referenceRows[0], (IEnumerable<byte>)image.LoadedNativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceRows[1], (IEnumerable<byte>)image.LoadedNativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceRows[2], (IEnumerable<byte>)image.LoadedNativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceRows[3], (IEnumerable<byte>)image.LoadedNativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceRows[4], (IEnumerable<byte>)image.LoadedNativeRows[4]);
            Assert.Equal((IEnumerable<byte>)referenceRows[5], (IEnumerable<byte>)image.LoadedNativeRows[5]);
        }

        [Fact]
        public void ValidOneTimesOne16Bit()
        {
            const string bodyString = "P3\n1 1 65535\n4660 22136 39612";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image16Factory();
            var reader = new NetpbmReader();
            NetpbmImage16 image;
            using (var bodyStream = new MemoryStream(bodyBytes, false))
            {
                image = (NetpbmImage16)reader.ReadImage(bodyStream, factory);
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(1, image.Header.Width);
            Assert.Equal(1, image.Header.Height);
            Assert.Equal(65535, image.Header.HighestComponentValue);
            Assert.Equal(3, image.Header.Components.Count);
            Assert.Equal(Component.Red, image.Header.Components[0]);
            Assert.Equal(Component.Green, image.Header.Components[1]);
            Assert.Equal(Component.Blue, image.Header.Components[2]);
            Assert.Equal(0x1234, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(0x5678, image.GetNativePixel(0, 0)[1]);
            Assert.Equal(0x9ABC, image.GetNativePixel(0, 0)[2]);
        }

        [Fact]
        public void TwoTimesTwoTooFewPixels()
        {
            const string bodyString = "P3\n2 2 1\n0 0 1 0 0 1\n0 0 1";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
        public void TwoTimesTwoTooFewComponents()
        {
            const string bodyString = "P3\n2 2 1\n0 0 1 0 0 1\n0 0 1 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

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
            const string bodyString = "P3\n2 2 1\n0 0 0 2 0 0 1 0 0 0 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }

        [Fact]
        public void TwoTimesTwoValueVeryOutOfRange()
        {
            const string bodyString = "P3\n2 2 1\n0 0 0 1048576 0 0 1 0 0 0 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }

        [Fact]
        public void TwoTimesTwoValueBlatantlyOutOfRange()
        {
            const string bodyString = "P3\n2 2 1\n0 0 0 36893488147419103232 0 0 1 0 0 0 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }

        [Fact]
        public void SixTimesSixGradientValueOutOfRange()
        {
            const string bodyString = "P3\n6 6 254\n255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0 0 0\n0 255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0 0\n0 0 255 0 0 204 0 0 153 0 0 102 0 0 51 0 0 0\n255 255 0 204 204 0 153 153 0 102 102 0 51 51 0 0 0 0\n255 0 255 204 0 204 153 0 153 102 0 102 51 0 51 0 0 0\n0 255 255 0 204 204 0 153 153 0 102 102 0 51 51 0 0 0";
            var bodyBytes = UsAsciiEncoding.GetBytes(bodyString);

            var factory = new ImageFactories.Image8Factory();
            var reader = new NetpbmReader();
            Assert.Throws<InvalidDataException>(() =>
            {
                using (var bodyStream = new MemoryStream(bodyBytes, false))
                {
                    var image = reader.ReadImage(bodyStream, factory);
                    image.LoadData();
                }
            });
        }
    }
}
