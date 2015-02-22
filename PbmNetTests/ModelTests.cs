using System;
using System.IO;
using RavuAlHemio.PbmNet;
using Xunit;
// ReSharper disable ObjectCreationAsStatement

namespace PbmNetTests
{
    public class ModelTests
    {
        [Fact]
        public void ValidSinglePixelBitmap()
        {
            var hdr = new NetpbmHeader<byte>(ImageType.PBM, 1, 1, 1, new[] { Component.Black }, 1);
            var img = new NetpbmImage8(hdr, new[] { new byte[] { 1 } });
            img.LoadData();

            Assert.Equal(1, img.Header.Width);
            Assert.Equal(1, img.Header.Height);
            Assert.Equal(1, img.Header.HighestComponentValue);
            Assert.Equal(1, img.Header.Components.Count);
            Assert.Equal(Component.Black, img.Header.Components[0]);
            Assert.Equal(1, img.LoadedNativeRows.Count);
            Assert.Equal(1, img.LoadedNativeRows[0].Count);
            Assert.Equal(1, img.LoadedNativeRows[0][0]);
            Assert.Equal(1, img.GetNativePixel(0, 0).Count);
            Assert.Equal(1, img.GetNativePixel(0, 0)[0]);
            Assert.Equal(1, img.GetScaledPixel(0, 0).Count);
            Assert.Equal(1.0, img.GetScaledPixel(0, 0)[0]);

            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetNativePixel(1, 0);
            });
            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetScaledPixel(1, 0);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetNativePixel(0, 1);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetScaledPixel(0, 1);
            });
        }

        [Fact]
        public void SinglePixelBitmapOutOfRangePixel()
        {
            var header = new NetpbmHeader<byte>(ImageType.PBM, 1, 1, 1, new[] {Component.Black}, 1);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] { new byte[] { 2 } });
                img.LoadData();
            });
        }

        [Fact]
        public void SinglePixelBitmapTooManyPixelsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PBM, 1, 1, 1, new[] { Component.Black }, 1);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] { new byte[] { 0, 1 } });
                img.LoadData();
            });
        }

        [Fact]
        public void SinglePixelBitmapTooManyRows()
        {
            var header = new NetpbmHeader<byte>(ImageType.PBM, 1, 1, 1, new[] { Component.Black }, 1);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] {new byte[] {0}, new byte[] {0}});
                img.LoadData();
            });
        }

        [Fact]
        public void ValidTwoTimesTwoGrayscale()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            var img = new NetpbmImage8(header,
                new[] {new byte[] {0, 64}, new byte[] {127, 64}});
            img.LoadData();

            Assert.Equal(2, img.Header.Width);
            Assert.Equal(2, img.Header.Height);
            Assert.Equal(127, img.Header.HighestComponentValue);
            Assert.Equal(1, img.Header.Components.Count);
            Assert.Equal(Component.Black, img.Header.Components[0]);

            Assert.Equal(2, img.LoadedNativeRows.Count);
            Assert.Equal(2, img.LoadedNativeRows[0].Count);
            Assert.Equal(2, img.LoadedNativeRows[1].Count);

            Assert.Equal(0, img.LoadedNativeRows[0][0]);
            Assert.Equal(64, img.LoadedNativeRows[0][1]);
            Assert.Equal(127, img.LoadedNativeRows[1][0]);
            Assert.Equal(64, img.LoadedNativeRows[1][1]);

            Assert.Equal(1, img.GetNativePixel(0, 0).Count);
            Assert.Equal(1, img.GetScaledPixel(0, 0).Count);
            Assert.Equal(0, img.GetNativePixel(0, 0)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 0)[0]);

            Assert.Equal(1, img.GetNativePixel(1, 0).Count);
            Assert.Equal(1, img.GetScaledPixel(1, 0).Count);
            Assert.Equal(64, img.GetNativePixel(1, 0)[0]);
            Assert.InRange(img.GetScaledPixel(1, 0)[0], 0.49, 0.51);

            Assert.Equal(1, img.GetNativePixel(0, 1).Count);
            Assert.Equal(1, img.GetScaledPixel(0, 1).Count);
            Assert.Equal(127, img.GetNativePixel(0, 1)[0]);
            Assert.Equal(1.0, img.GetScaledPixel(0, 1)[0]);

            Assert.Equal(1, img.GetNativePixel(1, 1).Count);
            Assert.Equal(1, img.GetScaledPixel(1, 1).Count);
            Assert.Equal(64, img.GetNativePixel(1, 1)[0]);
            Assert.InRange(img.GetScaledPixel(1, 1)[0], 0.49, 0.51);

            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetNativePixel(2, 0);
            });

            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetScaledPixel(2, 0);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetNativePixel(0, 2);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetScaledPixel(0, 2);
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleOutOfRangePixel()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] { new byte[] { 0, 64 }, new byte[] { 128, 64 } });
                img.LoadData();
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooManyRows()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header,
                    new[] { new byte[] { 0, 64 }, new byte[] { 127, 64 }, new byte[] { 0, 64 } });
                img.LoadData();
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooFewRows()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] { new byte[] { 0, 64 } });
                img.LoadData();
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooManyPixelsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header,
                    new[] { new byte[] { 0, 64 }, new byte[] { 127, 64, 127 } });
                img.LoadData();
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooFewPixelsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PGM, 2, 2, 1, new[] { Component.Black }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(header, new[] { new byte[] { 0, 64 }, new byte[] { 127 } });
                img.LoadData();
            });
        }

        [Fact]
        public void ValidThreeTimesThreeRGB()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            var img = new NetpbmImage8(
                header,
                new[]
                {
                    new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                    new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0 },
                    new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                });
            img.LoadData();

            Assert.Equal(3, img.Header.Width);
            Assert.Equal(3, img.Header.Height);
            Assert.Equal(255, img.Header.HighestComponentValue);
            Assert.Equal(3, img.Header.Components.Count);
            Assert.Equal(Component.Red, img.Header.Components[0]);
            Assert.Equal(Component.Green, img.Header.Components[1]);
            Assert.Equal(Component.Blue, img.Header.Components[2]);

            Assert.Equal(3, img.LoadedNativeRows.Count);
            Assert.Equal(9, img.LoadedNativeRows[0].Count);
            Assert.Equal(9, img.LoadedNativeRows[1].Count);

            Assert.Equal(255, img.LoadedNativeRows[0][0]);
            Assert.Equal(0, img.LoadedNativeRows[0][1]);
            Assert.Equal(0, img.LoadedNativeRows[0][2]);
            Assert.Equal(255, img.LoadedNativeRows[0][3]);
            Assert.Equal(0, img.LoadedNativeRows[0][4]);
            Assert.Equal(0, img.LoadedNativeRows[0][5]);
            Assert.Equal(255, img.LoadedNativeRows[0][6]);
            Assert.Equal(0, img.LoadedNativeRows[0][7]);
            Assert.Equal(0, img.LoadedNativeRows[0][8]);

            Assert.Equal(0, img.LoadedNativeRows[1][0]);
            Assert.Equal(255, img.LoadedNativeRows[1][1]);
            Assert.Equal(0, img.LoadedNativeRows[1][2]);
            Assert.Equal(0, img.LoadedNativeRows[1][3]);
            Assert.Equal(255, img.LoadedNativeRows[1][4]);
            Assert.Equal(0, img.LoadedNativeRows[1][5]);
            Assert.Equal(0, img.LoadedNativeRows[1][6]);
            Assert.Equal(255, img.LoadedNativeRows[1][7]);
            Assert.Equal(0, img.LoadedNativeRows[1][8]);

            Assert.Equal(0, img.LoadedNativeRows[2][0]);
            Assert.Equal(0, img.LoadedNativeRows[2][1]);
            Assert.Equal(255, img.LoadedNativeRows[2][2]);
            Assert.Equal(0, img.LoadedNativeRows[2][3]);
            Assert.Equal(0, img.LoadedNativeRows[2][4]);
            Assert.Equal(255, img.LoadedNativeRows[2][5]);
            Assert.Equal(0, img.LoadedNativeRows[2][6]);
            Assert.Equal(0, img.LoadedNativeRows[2][7]);
            Assert.Equal(255, img.LoadedNativeRows[2][8]);

            Assert.Equal(3, img.GetNativePixel(0, 0).Count);
            Assert.Equal(3, img.GetScaledPixel(0, 0).Count);
            Assert.Equal(255, img.GetNativePixel(0, 0)[0]);
            Assert.Equal(1.0, img.GetScaledPixel(0, 0)[0]);
            Assert.Equal(0, img.GetNativePixel(0, 0)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 0)[1]);
            Assert.Equal(0, img.GetNativePixel(0, 0)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 0)[2]);

            Assert.Equal(3, img.GetNativePixel(1, 0).Count);
            Assert.Equal(3, img.GetScaledPixel(1, 0).Count);
            Assert.Equal(255, img.GetNativePixel(1, 0)[0]);
            Assert.Equal(1.0, img.GetScaledPixel(1, 0)[0]);
            Assert.Equal(0, img.GetNativePixel(1, 0)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 0)[1]);
            Assert.Equal(0, img.GetNativePixel(1, 0)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 0)[2]);

            Assert.Equal(3, img.GetNativePixel(2, 0).Count);
            Assert.Equal(3, img.GetScaledPixel(2, 0).Count);
            Assert.Equal(255, img.GetNativePixel(2, 0)[0]);
            Assert.Equal(1.0, img.GetScaledPixel(2, 0)[0]);
            Assert.Equal(0, img.GetNativePixel(2, 0)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 0)[1]);
            Assert.Equal(0, img.GetNativePixel(2, 0)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 0)[2]);

            Assert.Equal(3, img.GetNativePixel(0, 1).Count);
            Assert.Equal(3, img.GetScaledPixel(0, 1).Count);
            Assert.Equal(0, img.GetNativePixel(0, 1)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 1)[0]);
            Assert.Equal(255, img.GetNativePixel(0, 1)[1]);
            Assert.Equal(1.0, img.GetScaledPixel(0, 1)[1]);
            Assert.Equal(0, img.GetNativePixel(0, 1)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 1)[2]);

            Assert.Equal(3, img.GetNativePixel(1, 1).Count);
            Assert.Equal(3, img.GetScaledPixel(1, 1).Count);
            Assert.Equal(0, img.GetNativePixel(1, 1)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 1)[0]);
            Assert.Equal(255, img.GetNativePixel(1, 1)[1]);
            Assert.Equal(1.0, img.GetScaledPixel(1, 1)[1]);
            Assert.Equal(0, img.GetNativePixel(1, 1)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 1)[2]);

            Assert.Equal(3, img.GetNativePixel(2, 1).Count);
            Assert.Equal(3, img.GetScaledPixel(2, 1).Count);
            Assert.Equal(0, img.GetNativePixel(2, 1)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 1)[0]);
            Assert.Equal(255, img.GetNativePixel(2, 1)[1]);
            Assert.Equal(1.0, img.GetScaledPixel(2, 1)[1]);
            Assert.Equal(0, img.GetNativePixel(2, 1)[2]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 1)[2]);

            Assert.Equal(3, img.GetNativePixel(0, 2).Count);
            Assert.Equal(3, img.GetScaledPixel(0, 2).Count);
            Assert.Equal(0, img.GetNativePixel(0, 2)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 2)[0]);
            Assert.Equal(0, img.GetNativePixel(0, 2)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(0, 2)[1]);
            Assert.Equal(255, img.GetNativePixel(0, 2)[2]);
            Assert.Equal(1.0, img.GetScaledPixel(0, 2)[2]);

            Assert.Equal(3, img.GetNativePixel(1, 2).Count);
            Assert.Equal(3, img.GetScaledPixel(1, 2).Count);
            Assert.Equal(0, img.GetNativePixel(1, 2)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 2)[0]);
            Assert.Equal(0, img.GetNativePixel(1, 2)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(1, 2)[1]);
            Assert.Equal(255, img.GetNativePixel(1, 2)[2]);
            Assert.Equal(1.0, img.GetScaledPixel(1, 2)[2]);

            Assert.Equal(3, img.GetNativePixel(2, 2).Count);
            Assert.Equal(3, img.GetScaledPixel(2, 2).Count);
            Assert.Equal(0, img.GetNativePixel(2, 2)[0]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 2)[0]);
            Assert.Equal(0, img.GetNativePixel(2, 2)[1]);
            Assert.Equal(0.0, img.GetScaledPixel(2, 2)[1]);
            Assert.Equal(255, img.GetNativePixel(2, 2)[2]);
            Assert.Equal(1.0, img.GetScaledPixel(2, 2)[2]);

            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetNativePixel(3, 0);
            });

            Assert.Throws<ArgumentOutOfRangeException>("x", () =>
            {
                img.GetScaledPixel(3, 0);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetNativePixel(0, 3);
            });

            Assert.Throws<ArgumentOutOfRangeException>("y", () =>
            {
                img.GetScaledPixel(0, 3);
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooFewPixelsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
                img.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooFewComponentsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var image = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
                image.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyPixelsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var image = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0, 0, 255, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
                image.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyComponentsInRow()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
                img.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBOutOfRangePixel()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 127);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 127, 0, 0, 127, 0, 0, 127, 0, 0 },
                        new byte[] { 0, 127, 0, 0, 127, 0, 0, 127 },
                        new byte[] { 0, 0, 127, 0, 0, 127, 0, 0, 128 }
                    });
                img.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooFewRows()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
                img.LoadData();
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyRows()
        {
            var header = new NetpbmHeader<byte>(ImageType.PPM, 3, 3, 1, new[] { Component.Red, Component.Green, Component.Blue }, 255);
            Assert.Throws<InvalidDataException>(() =>
            {
                var img = new NetpbmImage8(
                    header,
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 255, 0, 255, 255, 0, 255, 255, 0, 255 }
                    });
                img.LoadData();
            });
        }
    }
}
