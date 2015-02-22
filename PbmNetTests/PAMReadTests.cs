using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RavuAlHemio.PbmNet;
using Xunit;

namespace PbmNetTests
{
    public class PAMReadTests
    {
        [Fact]
        public void ValidTwoTimesTwoBitmap()
        {
            var bodyRawBytes = new object[]
            {
                'P', '7',
                '\n',
                'W', 'I', 'D', 'T', 'H', ' ', '2', '\n',
                'H', 'E', 'I', 'G', 'H', 'T', ' ', '2', '\n',
                'D', 'E', 'P', 'T', 'H', ' ', '1', '\n',
                'M', 'A', 'X', 'V', 'A', 'L', ' ', '1', '\n',
                'T', 'U', 'P', 'L', 'T', 'Y', 'P', 'E', ' ', 'B', 'L', 'A', 'C', 'K', 'A', 'N', 'D', 'W', 'H', 'I', 'T', 'E', '\n',
                'E', 'N', 'D', 'H', 'D', 'R', '\n',
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
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(2, image.Header.Width);
            Assert.Equal(2, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(1, image.Header.Components.Count);
            Assert.Equal(Component.White, image.Header.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidTwoTimesTwoBitmapWithComments()
        {
            var bodyRawBytes = new object[]
            {
                'P', '7',
                '\n',
                '#', ' ', 't', 'w', 'o', ' ', 't', 'i', 'm', 'e', 's', ' ', 't', 'w', 'o', '\n',
                'W', 'I', 'D', 'T', 'H', ' ', '2', '\n',
                'H', 'E', 'I', 'G', 'H', 'T', ' ', '2', '\n',
                '#', ' ', 's', 'i', 'n', 'g', 'l', 'e', ' ', 'p', 'l', 'a', 'n', 'e', '\n',
                'D', 'E', 'P', 'T', 'H', ' ', '1', '\n',
                '#', ' ', 'b', 'i', 't', 'm', 'a', 'p', '\n',
                'M', 'A', 'X', 'V', 'A', 'L', ' ', '1', '\n',
                'T', 'U', 'P', 'L', 'T', 'Y', 'P', 'E', ' ', 'B', 'L', 'A', 'C', 'K', 'A', 'N', 'D', 'W', 'H', 'I', 'T', 'E', '\n',
                '#', ' ', 'd', 'o', 'n', 'e', '\n',
                'E', 'N', 'D', 'H', 'D', 'R', '\n',
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
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(2, image.Header.Width);
            Assert.Equal(2, image.Header.Height);
            Assert.Equal(1, image.Header.HighestComponentValue);
            Assert.Equal(1, image.Header.Components.Count);
            Assert.Equal(Component.White, image.Header.Components[0]);
            Assert.Equal(0, image.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(1, 0)[0]);
            Assert.Equal(1, image.GetNativePixel(0, 1)[0]);
            Assert.Equal(0, image.GetNativePixel(1, 1)[0]);
        }

        [Fact]
        public void ValidSixTimesSixGrayscale()
        {
            var referenceRow = new byte[] { 255, 204, 153, 102, 51, 0 };
            var bodyRawBytes = new object[]
            {
                'P', '7',
                '\n',
                'W', 'I', 'D', 'T', 'H', ' ', '6', '\n',
                'H', 'E', 'I', 'G', 'H', 'T', ' ', '6', '\n',
                'D', 'E', 'P', 'T', 'H', ' ', '1', '\n',
                'M', 'A', 'X', 'V', 'A', 'L', ' ', '2', '5', '5', '\n',
                'T', 'U', 'P', 'L', 'T', 'Y', 'P', 'E', ' ', 'G', 'R', 'A', 'Y', 'S', 'C', 'A', 'L', 'E', '\n',
                'E', 'N', 'D', 'H', 'D', 'R', '\n',
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
                image.LoadData();
                Assert.Equal(-1, bodyStream.ReadByte());
            }

            Assert.Equal(6, image.Header.Width);
            Assert.Equal(6, image.Header.Height);
            Assert.Equal(255, image.Header.HighestComponentValue);
            Assert.Equal(1, image.Header.Components.Count);
            Assert.Equal(Component.White, image.Header.Components[0]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[4]);
            Assert.Equal((IEnumerable<byte>)referenceRow, (IEnumerable<byte>)image.LoadedNativeRows[5]);
        }
    }
}
