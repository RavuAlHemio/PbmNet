using System.Collections.Generic;
using RavuAlHemio.PbmNet;
using Xunit;
// ReSharper disable RedundantCast

namespace PbmNetTests
{
    public class CanonicalizerTests
    {
        [Fact]
        public void SinglePixelBitmapAdditiveToSubtractive()
        {
            var subtractive = new NetpbmImage8(1, 1, 1, new[] {Component.Black}, new[] {new byte[] {1}});
            var canon = new Canonicalizer();
            var additive = canon.Canonicalize(subtractive, GrayscaleConversion.BlackToWhite);

            Assert.Equal(1, additive.Width);
            Assert.Equal(1, additive.Height);
            Assert.Equal(1, additive.HighestComponentValue);
            Assert.Equal(1, additive.Components.Count);
            Assert.Equal(Component.White, additive.Components[0]);
            Assert.Equal(1, additive.NativeRows.Count);
            Assert.Equal(1, additive.NativeRows[0].Count);
            Assert.Equal(0, additive.NativeRows[0][0]);
        }

        [Fact]
        public void SinglePixelBitmapSubtractiveToAdditive()
        {
            var additive = new NetpbmImage8(1, 1, 1, new[] {Component.White}, new[] {new byte[] {1}});
            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);

            Assert.Equal(1, subtractive.Width);
            Assert.Equal(1, subtractive.Height);
            Assert.Equal(1, subtractive.HighestComponentValue);
            Assert.Equal(1, subtractive.Components.Count);
            Assert.Equal(Component.Black, subtractive.Components[0]);
            Assert.Equal(1, subtractive.NativeRows.Count);
            Assert.Equal(1, subtractive.NativeRows[0].Count);
            Assert.Equal(0, subtractive.NativeRows[0][0]);
        }

        [Fact]
        public void ThreeTimesThreeGrayscaleSubtractiveToAdditive()
        {
            var subtractive = new NetpbmImage8(3, 3, 255, new[] { Component.Black }, new[]
            {
                new byte[] { 12, 34, 56 },
                new byte[] { 78, 90, 12 },
                new byte[] { 34, 56, 78 }
            });
            var canon = new Canonicalizer();
            var additive = canon.Canonicalize(subtractive, GrayscaleConversion.BlackToWhite);

            Assert.Equal(3, additive.Width);
            Assert.Equal(3, additive.Height);
            Assert.Equal(255, additive.HighestComponentValue);
            Assert.Equal(1, additive.Components.Count);
            Assert.Equal(Component.White, additive.Components[0]);
            Assert.Equal(3, additive.NativeRows.Count);
            Assert.Equal(3, additive.NativeRows[0].Count);
            Assert.Equal(3, additive.NativeRows[1].Count);
            Assert.Equal(3, additive.NativeRows[2].Count);
            Assert.Equal(243, additive.NativeRows[0][0]);
            Assert.Equal(221, additive.NativeRows[0][1]);
            Assert.Equal(199, additive.NativeRows[0][2]);
            Assert.Equal(177, additive.NativeRows[1][0]);
            Assert.Equal(165, additive.NativeRows[1][1]);
            Assert.Equal(243, additive.NativeRows[1][2]);
            Assert.Equal(221, additive.NativeRows[2][0]);
            Assert.Equal(199, additive.NativeRows[2][1]);
            Assert.Equal(177, additive.NativeRows[2][2]);
        }

        [Fact]
        public void ThreeTimesThreeGrayscaleAdditiveToSubtractive()
        {
            var additive = new NetpbmImage8(3, 3, 255, new[] { Component.White }, new[]
            {
                new byte[] { 12, 34, 56 },
                new byte[] { 78, 90, 12 },
                new byte[] { 34, 56, 78 }
            });
            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);

            Assert.Equal(3, subtractive.Width);
            Assert.Equal(3, subtractive.Height);
            Assert.Equal(255, subtractive.HighestComponentValue);
            Assert.Equal(1, subtractive.Components.Count);
            Assert.Equal(Component.Black, subtractive.Components[0]);
            Assert.Equal(3, subtractive.NativeRows.Count);
            Assert.Equal(3, subtractive.NativeRows[0].Count);
            Assert.Equal(3, subtractive.NativeRows[1].Count);
            Assert.Equal(3, subtractive.NativeRows[2].Count);
            Assert.Equal(243, subtractive.NativeRows[0][0]);
            Assert.Equal(221, subtractive.NativeRows[0][1]);
            Assert.Equal(199, subtractive.NativeRows[0][2]);
            Assert.Equal(177, subtractive.NativeRows[1][0]);
            Assert.Equal(165, subtractive.NativeRows[1][1]);
            Assert.Equal(243, subtractive.NativeRows[1][2]);
            Assert.Equal(221, subtractive.NativeRows[2][0]);
            Assert.Equal(199, subtractive.NativeRows[2][1]);
            Assert.Equal(177, subtractive.NativeRows[2][2]);
        }

        [Fact]
        public void FiveTimesFiveCMYWToCMYK()
        {
            var additive = new NetpbmImage8(5, 5, 255, new[]
            {
                Component.Cyan, Component.Magenta, Component.Yellow, Component.White
            }, new[]
            {
                new byte[]
                {
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90
                },
                new byte[]
                {
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12
                },
                new byte[]
                {
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34
                },
                new byte[]
                {
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56
                },
                new byte[]
                {
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78
                }
            });

            var referenceValues = new[]
            {
                new byte[]
                {
                    12, 34, 56, 177,
                    90, 12, 34, 199,
                    78, 90, 12, 221,
                    56, 78, 90, 243,
                    34, 56, 78, 165
                },
                new byte[]
                {
                    34, 56, 78, 165,
                    12, 34, 56, 177,
                    90, 12, 34, 199,
                    78, 90, 12, 221,
                    56, 78, 90, 243
                },
                new byte[]
                {
                    56, 78, 90, 243,
                    34, 56, 78, 165,
                    12, 34, 56, 177,
                    90, 12, 34, 199,
                    78, 90, 12, 221
                },
                new byte[]
                {
                    78, 90, 12, 221,
                    56, 78, 90, 243,
                    34, 56, 78, 165,
                    12, 34, 56, 177,
                    90, 12, 34, 199
                },
                new byte[]
                {
                    90, 12, 34, 199,
                    78, 90, 12, 221,
                    56, 78, 90, 243,
                    34, 56, 78, 165,
                    12, 34, 56, 177
                }
            };

            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);

            Assert.Equal(5, subtractive.Width);
            Assert.Equal(5, subtractive.Height);
            Assert.Equal(255, subtractive.HighestComponentValue);
            Assert.Equal(4, subtractive.Components.Count);
            Assert.Equal(Component.Cyan, subtractive.Components[0]);
            Assert.Equal(Component.Magenta, subtractive.Components[1]);
            Assert.Equal(Component.Yellow, subtractive.Components[2]);
            Assert.Equal(Component.Black, subtractive.Components[3]);
            Assert.Equal(5, subtractive.NativeRows.Count);
            Assert.Equal(20, subtractive.NativeRows[0].Count);
            Assert.Equal(20, subtractive.NativeRows[1].Count);
            Assert.Equal(20, subtractive.NativeRows[2].Count);
            Assert.Equal(20, subtractive.NativeRows[3].Count);
            Assert.Equal(20, subtractive.NativeRows[4].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)subtractive.NativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)subtractive.NativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)subtractive.NativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceValues[3], (IEnumerable<byte>)subtractive.NativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceValues[4], (IEnumerable<byte>)subtractive.NativeRows[4]);
        }

        [Fact]
        public void ThreeTimesThreeBGRToRGB()
        {
            var bgr = new NetpbmImage8(3, 3, 255, new[]
            {
                Component.Blue, Component.Green, Component.Red
            }, new[]
            {
                new byte[]
                {
                    255, 0, 0,
                    0, 255, 0,
                    0, 0, 255
                },
                new byte[]
                {
                    0, 255, 0,
                    0, 0, 255,
                    255, 0, 0
                },
                new byte[]
                {
                    0, 0, 255,
                    255, 0, 0,
                    0, 255, 0
                }
            });

            var referenceValues = new[]
            {
                new byte[]
                {
                    0, 0, 255,
                    0, 255, 0,
                    255, 0, 0
                },
                new byte[]
                {
                    0, 255, 0,
                    255, 0, 0,
                    0, 0, 255
                },
                new byte[]
                {
                    255, 0, 0,
                    0, 0, 255,
                    0, 255, 0
                }
            };

            var canon = new Canonicalizer();
            var rgb = canon.Canonicalize(bgr, GrayscaleConversion.WhiteToBlack);

            Assert.Equal(3, rgb.Width);
            Assert.Equal(3, rgb.Height);
            Assert.Equal(255, rgb.HighestComponentValue);
            Assert.Equal(3, rgb.Components.Count);
            Assert.Equal(Component.Red, rgb.Components[0]);
            Assert.Equal(Component.Green, rgb.Components[1]);
            Assert.Equal(Component.Blue, rgb.Components[2]);
            Assert.Equal(3, rgb.NativeRows.Count);
            Assert.Equal(9, rgb.NativeRows[0].Count);
            Assert.Equal(9, rgb.NativeRows[1].Count);
            Assert.Equal(9, rgb.NativeRows[2].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)rgb.NativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)rgb.NativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)rgb.NativeRows[2]);
        }

        [Fact]
        public void FiveTimesFiveWCMYToCMYK()
        {
            var additive = new NetpbmImage8(5, 5, 255, new[]
            {
                Component.White, Component.Cyan, Component.Magenta, Component.Yellow
            }, new[]
            {
                new byte[]
                {
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90
                },
                new byte[]
                {
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12
                },
                new byte[]
                {
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56,
                    78, 90, 12, 34
                },
                new byte[]
                {
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78,
                    90, 12, 34, 56
                },
                new byte[]
                {
                    90, 12, 34, 56,
                    78, 90, 12, 34,
                    56, 78, 90, 12,
                    34, 56, 78, 90,
                    12, 34, 56, 78
                }
            });

            var referenceValues = new[]
            {
                new byte[]
                {
                    34, 56, 78, 243,
                    12, 34, 56, 165,
                    90, 12, 34, 177,
                    78, 90, 12, 199,
                    56, 78, 90, 221
                },
                new byte[]
                {
                    56, 78, 90, 221,
                    34, 56, 78, 243,
                    12, 34, 56, 165,
                    90, 12, 34, 177,
                    78, 90, 12, 199
                },
                new byte[]
                {
                    78, 90, 12, 199,
                    56, 78, 90, 221,
                    34, 56, 78, 243,
                    12, 34, 56, 165,
                    90, 12, 34, 177
                },
                new byte[]
                {
                    90, 12, 34, 177,
                    78, 90, 12, 199,
                    56, 78, 90, 221,
                    34, 56, 78, 243,
                    12, 34, 56, 165
                },
                new byte[]
                {
                    12, 34, 56, 165,
                    90, 12, 34, 177,
                    78, 90, 12, 199,
                    56, 78, 90, 221,
                    34, 56, 78, 243
                }
            };

            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);

            Assert.Equal(5, subtractive.Width);
            Assert.Equal(5, subtractive.Height);
            Assert.Equal(255, subtractive.HighestComponentValue);
            Assert.Equal(4, subtractive.Components.Count);
            Assert.Equal(Component.Cyan, subtractive.Components[0]);
            Assert.Equal(Component.Magenta, subtractive.Components[1]);
            Assert.Equal(Component.Yellow, subtractive.Components[2]);
            Assert.Equal(Component.Black, subtractive.Components[3]);
            Assert.Equal(5, subtractive.NativeRows.Count);
            Assert.Equal(20, subtractive.NativeRows[0].Count);
            Assert.Equal(20, subtractive.NativeRows[1].Count);
            Assert.Equal(20, subtractive.NativeRows[2].Count);
            Assert.Equal(20, subtractive.NativeRows[3].Count);
            Assert.Equal(20, subtractive.NativeRows[4].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)subtractive.NativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)subtractive.NativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)subtractive.NativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceValues[3], (IEnumerable<byte>)subtractive.NativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceValues[4], (IEnumerable<byte>)subtractive.NativeRows[4]);
        }
    }
}
