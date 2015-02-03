using System;
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
            var img = new NetpbmImage8(1, 1, 1, new[] {Component.Black}, new[] {new byte[] {1}});
            Assert.Equal(1, img.Width);
            Assert.Equal(1, img.Height);
            Assert.True(img.IsBitmap);
            Assert.Equal(1, img.HighestComponentValue);
            Assert.Equal(1, img.Components.Count);
            Assert.Equal(Component.Black, img.Components[0]);
            Assert.Equal(1, img.NativeRows.Count);
            Assert.Equal(1, img.NativeRows[0].Count);
            Assert.Equal(1, img.NativeRows[0][0]);
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
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () => {
                new NetpbmImage8(1, 1, 1, new[] {Component.Black}, new[] {new byte[] {2}});
            });
        }

        [Fact]
        public void SinglePixelBitmapTooManyPixelsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(1, 1, 1, new[] {Component.Black}, new[] {new byte[] {0, 1}});
            });
        }

        [Fact]
        public void SinglePixelBitmapTooManyRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(1, 1, 1, new[] {Component.Black}, new[] {new byte[] {0}, new byte[] {0}});
            });
        }

        [Fact]
        public void ValidTwoTimesTwoGrayscale()
        {
            var img = new NetpbmImage8(2, 2, 127, new[] {Component.Black},
                new[] {new byte[] {0, 64}, new byte[] {127, 64}});
            Assert.Equal(2, img.Width);
            Assert.Equal(2, img.Height);
            Assert.False(img.IsBitmap);
            Assert.Equal(127, img.HighestComponentValue);
            Assert.Equal(1, img.Components.Count);
            Assert.Equal(Component.Black, img.Components[0]);

            Assert.Equal(2, img.NativeRows.Count);
            Assert.Equal(2, img.NativeRows[0].Count);
            Assert.Equal(2, img.NativeRows[1].Count);

            Assert.Equal(0, img.NativeRows[0][0]);
            Assert.Equal(64, img.NativeRows[0][1]);
            Assert.Equal(127, img.NativeRows[1][0]);
            Assert.Equal(64, img.NativeRows[1][1]);

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
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(2, 2, 127, new[] {Component.Black}, new[] {new byte[] {0, 64}, new byte[] {128, 64}});
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooManyRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(2, 2, 127, new[] {Component.Black},
                    new[] {new byte[] {0, 64}, new byte[] {127, 64}, new byte[] {0, 64}});
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooFewRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(2, 2, 127, new[] {Component.Black}, new[] {new byte[] {0, 64}});
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooManyPixelsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(2, 2, 127, new[] {Component.Black},
                    new[] {new byte[] {0, 64}, new byte[] {127, 64, 127}});
            });
        }

        [Fact]
        public void TwoTimesTwoGrayscaleTooFewPixelsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(2, 2, 127, new[] {Component.Black}, new[] {new byte[] {0, 64}, new byte[] {127}});
            });
        }

        [Fact]
        public void ValidThreeTimesThreeRGB()
        {
            var img = new NetpbmImage8(
                3, 3,
                255,
                new[] { Component.Red, Component.Green, Component.Blue },
                new[]
                {
                    new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                    new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0 },
                    new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                });

            Assert.Equal(3, img.Width);
            Assert.Equal(3, img.Height);
            Assert.False(img.IsBitmap);
            Assert.Equal(255, img.HighestComponentValue);
            Assert.Equal(3, img.Components.Count);
            Assert.Equal(Component.Red, img.Components[0]);
            Assert.Equal(Component.Green, img.Components[1]);
            Assert.Equal(Component.Blue, img.Components[2]);

            Assert.Equal(3, img.NativeRows.Count);
            Assert.Equal(9, img.NativeRows[0].Count);
            Assert.Equal(9, img.NativeRows[1].Count);

            Assert.Equal(255, img.NativeRows[0][0]);
            Assert.Equal(0, img.NativeRows[0][1]);
            Assert.Equal(0, img.NativeRows[0][2]);
            Assert.Equal(255, img.NativeRows[0][3]);
            Assert.Equal(0, img.NativeRows[0][4]);
            Assert.Equal(0, img.NativeRows[0][5]);
            Assert.Equal(255, img.NativeRows[0][6]);
            Assert.Equal(0, img.NativeRows[0][7]);
            Assert.Equal(0, img.NativeRows[0][8]);

            Assert.Equal(0, img.NativeRows[1][0]);
            Assert.Equal(255, img.NativeRows[1][1]);
            Assert.Equal(0, img.NativeRows[1][2]);
            Assert.Equal(0, img.NativeRows[1][3]);
            Assert.Equal(255, img.NativeRows[1][4]);
            Assert.Equal(0, img.NativeRows[1][5]);
            Assert.Equal(0, img.NativeRows[1][6]);
            Assert.Equal(255, img.NativeRows[1][7]);
            Assert.Equal(0, img.NativeRows[1][8]);

            Assert.Equal(0, img.NativeRows[2][0]);
            Assert.Equal(0, img.NativeRows[2][1]);
            Assert.Equal(255, img.NativeRows[2][2]);
            Assert.Equal(0, img.NativeRows[2][3]);
            Assert.Equal(0, img.NativeRows[2][4]);
            Assert.Equal(255, img.NativeRows[2][5]);
            Assert.Equal(0, img.NativeRows[2][6]);
            Assert.Equal(0, img.NativeRows[2][7]);
            Assert.Equal(255, img.NativeRows[2][8]);

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
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooFewComponentsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyPixelsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0, 0, 255, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyComponentsInRow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBOutOfRangePixel()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    127,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 127, 0, 0, 127, 0, 0, 127, 0, 0 },
                        new byte[] { 0, 127, 0, 0, 127, 0, 0, 127 },
                        new byte[] { 0, 0, 127, 0, 0, 127, 0, 0, 128 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooFewRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 }
                    });
            });
        }

        [Fact]
        public void ThreeTimesThreeRGBTooManyRows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("pixelData", () =>
            {
                new NetpbmImage8(
                    3, 3,
                    255,
                    new[] { Component.Red, Component.Green, Component.Blue },
                    new[]
                    {
                        new byte[] { 255, 0, 0, 255, 0, 0, 255, 0, 0 },
                        new byte[] { 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 0, 0, 255, 0, 0, 255, 0, 0, 255 },
                        new byte[] { 255, 0, 255, 255, 0, 255, 255, 0, 255 }
                    });
            });
        }
    }
}
