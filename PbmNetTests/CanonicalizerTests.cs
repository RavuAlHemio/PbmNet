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
            var subtractiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 1, 1, 1, new[] {Component.Black}, 1);
            var subtractive = new NetpbmImage8(subtractiveHeader, new[] {new byte[] {1}});
            var canon = new Canonicalizer();
            var additive = canon.Canonicalize(subtractive, GrayscaleConversion.BlackToWhite);
            additive.LoadData();

            Assert.Equal(1, additive.Header.Width);
            Assert.Equal(1, additive.Header.Height);
            Assert.Equal(1, additive.Header.HighestComponentValue);
            Assert.Equal(1, additive.Header.Components.Count);
            Assert.Equal(Component.White, additive.Header.Components[0]);
            Assert.NotNull(additive.LoadedNativeRows);
            Assert.Equal(1, additive.LoadedNativeRows.Count);
            Assert.Equal(1, additive.LoadedNativeRows[0].Count);
            Assert.Equal(0, additive.LoadedNativeRows[0][0]);
        }

        [Fact]
        public void SinglePixelBitmapSubtractiveToAdditive()
        {
            var additiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 1, 1, 1, new[] { Component.White }, 1);
            var additive = new NetpbmImage8(additiveHeader, new[] { new byte[] { 1 } });
            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);
            subtractive.LoadData();

            Assert.Equal(1, subtractive.Header.Width);
            Assert.Equal(1, subtractive.Header.Height);
            Assert.Equal(1, subtractive.Header.HighestComponentValue);
            Assert.Equal(1, subtractive.Header.Components.Count);
            Assert.Equal(Component.Black, subtractive.Header.Components[0]);
            Assert.NotNull(subtractive.LoadedNativeRows);
            Assert.Equal(1, subtractive.LoadedNativeRows.Count);
            Assert.Equal(1, subtractive.LoadedNativeRows[0].Count);
            Assert.Equal(0, subtractive.LoadedNativeRows[0][0]);
        }

        [Fact]
        public void ThreeTimesThreeGrayscaleSubtractiveToAdditive()
        {
            var subtractiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 3, 3, 1, new[] {Component.Black}, 255);
            var subtractive = new NetpbmImage8(subtractiveHeader, new[]
            {
                new byte[] { 12, 34, 56 },
                new byte[] { 78, 90, 12 },
                new byte[] { 34, 56, 78 }
            });
            var canon = new Canonicalizer();
            var additive = canon.Canonicalize(subtractive, GrayscaleConversion.BlackToWhite);
            additive.LoadData();

            Assert.Equal(3, additive.Header.Width);
            Assert.Equal(3, additive.Header.Height);
            Assert.Equal(255, additive.Header.HighestComponentValue);
            Assert.Equal(1, additive.Header.Components.Count);
            Assert.Equal(Component.White, additive.Header.Components[0]);
            Assert.NotNull(additive.LoadedNativeRows);
            Assert.Equal(3, additive.LoadedNativeRows.Count);
            Assert.Equal(3, additive.LoadedNativeRows[0].Count);
            Assert.Equal(3, additive.LoadedNativeRows[1].Count);
            Assert.Equal(3, additive.LoadedNativeRows[2].Count);
            Assert.Equal(243, additive.LoadedNativeRows[0][0]);
            Assert.Equal(221, additive.LoadedNativeRows[0][1]);
            Assert.Equal(199, additive.LoadedNativeRows[0][2]);
            Assert.Equal(177, additive.LoadedNativeRows[1][0]);
            Assert.Equal(165, additive.LoadedNativeRows[1][1]);
            Assert.Equal(243, additive.LoadedNativeRows[1][2]);
            Assert.Equal(221, additive.LoadedNativeRows[2][0]);
            Assert.Equal(199, additive.LoadedNativeRows[2][1]);
            Assert.Equal(177, additive.LoadedNativeRows[2][2]);
        }

        [Fact]
        public void ThreeTimesThreeGrayscaleAdditiveToSubtractive()
        {
            var additiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 3, 3, 1, new[] { Component.White }, 255);
            var additive = new NetpbmImage8(additiveHeader, new[]
            {
                new byte[] { 12, 34, 56 },
                new byte[] { 78, 90, 12 },
                new byte[] { 34, 56, 78 }
            });
            var canon = new Canonicalizer();
            var subtractive = canon.Canonicalize(additive, GrayscaleConversion.WhiteToBlack);
            subtractive.LoadData();

            Assert.Equal(3, subtractive.Header.Width);
            Assert.Equal(3, subtractive.Header.Height);
            Assert.Equal(255, subtractive.Header.HighestComponentValue);
            Assert.Equal(1, subtractive.Header.Components.Count);
            Assert.Equal(Component.Black, subtractive.Header.Components[0]);
            Assert.NotNull(subtractive.LoadedNativeRows);
            Assert.Equal(3, subtractive.LoadedNativeRows.Count);
            Assert.Equal(3, subtractive.LoadedNativeRows[0].Count);
            Assert.Equal(3, subtractive.LoadedNativeRows[1].Count);
            Assert.Equal(3, subtractive.LoadedNativeRows[2].Count);
            Assert.Equal(243, subtractive.LoadedNativeRows[0][0]);
            Assert.Equal(221, subtractive.LoadedNativeRows[0][1]);
            Assert.Equal(199, subtractive.LoadedNativeRows[0][2]);
            Assert.Equal(177, subtractive.LoadedNativeRows[1][0]);
            Assert.Equal(165, subtractive.LoadedNativeRows[1][1]);
            Assert.Equal(243, subtractive.LoadedNativeRows[1][2]);
            Assert.Equal(221, subtractive.LoadedNativeRows[2][0]);
            Assert.Equal(199, subtractive.LoadedNativeRows[2][1]);
            Assert.Equal(177, subtractive.LoadedNativeRows[2][2]);
        }

        [Fact]
        public void FiveTimesFiveCMYWToCMYK()
        {
            var additiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 5, 5, 1, new[] { Component.Cyan, Component.Magenta, Component.Yellow, Component.White }, 255);
            var additive = new NetpbmImage8(additiveHeader, new[]
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
            subtractive.LoadData();

            Assert.Equal(5, subtractive.Header.Width);
            Assert.Equal(5, subtractive.Header.Height);
            Assert.Equal(255, subtractive.Header.HighestComponentValue);
            Assert.Equal(4, subtractive.Header.Components.Count);
            Assert.Equal(Component.Cyan, subtractive.Header.Components[0]);
            Assert.Equal(Component.Magenta, subtractive.Header.Components[1]);
            Assert.Equal(Component.Yellow, subtractive.Header.Components[2]);
            Assert.Equal(Component.Black, subtractive.Header.Components[3]);
            Assert.NotNull(subtractive.LoadedNativeRows);
            Assert.Equal(5, subtractive.LoadedNativeRows.Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[0].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[1].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[2].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[3].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[4].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)subtractive.LoadedNativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)subtractive.LoadedNativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)subtractive.LoadedNativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceValues[3], (IEnumerable<byte>)subtractive.LoadedNativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceValues[4], (IEnumerable<byte>)subtractive.LoadedNativeRows[4]);
        }

        [Fact]
        public void ThreeTimesThreeBGRToRGB()
        {
            var bgrHeader = new NetpbmHeader<byte>(ImageType.PAM, 3, 3, 1, new[] { Component.Blue, Component.Green, Component.Red }, 255);
            var bgr = new NetpbmImage8(bgrHeader, new[]
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
            rgb.LoadData();

            Assert.Equal(3, rgb.Header.Width);
            Assert.Equal(3, rgb.Header.Height);
            Assert.Equal(255, rgb.Header.HighestComponentValue);
            Assert.Equal(3, rgb.Header.Components.Count);
            Assert.Equal(Component.Red, rgb.Header.Components[0]);
            Assert.Equal(Component.Green, rgb.Header.Components[1]);
            Assert.Equal(Component.Blue, rgb.Header.Components[2]);
            Assert.NotNull(rgb.LoadedNativeRows);
            Assert.Equal(3, rgb.LoadedNativeRows.Count);
            Assert.Equal(9, rgb.LoadedNativeRows[0].Count);
            Assert.Equal(9, rgb.LoadedNativeRows[1].Count);
            Assert.Equal(9, rgb.LoadedNativeRows[2].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)rgb.LoadedNativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)rgb.LoadedNativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)rgb.LoadedNativeRows[2]);
        }

        [Fact]
        public void FiveTimesFiveWCMYToCMYK()
        {
            var additiveHeader = new NetpbmHeader<byte>(ImageType.PAM, 5, 5, 1, new[] { Component.White, Component.Cyan, Component.Magenta, Component.Yellow }, 255);
            var additive = new NetpbmImage8(additiveHeader, new[]
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
            subtractive.LoadData();

            Assert.Equal(5, subtractive.Header.Width);
            Assert.Equal(5, subtractive.Header.Height);
            Assert.Equal(255, subtractive.Header.HighestComponentValue);
            Assert.Equal(4, subtractive.Header.Components.Count);
            Assert.Equal(Component.Cyan, subtractive.Header.Components[0]);
            Assert.Equal(Component.Magenta, subtractive.Header.Components[1]);
            Assert.Equal(Component.Yellow, subtractive.Header.Components[2]);
            Assert.Equal(Component.Black, subtractive.Header.Components[3]);
            Assert.NotNull(subtractive.LoadedNativeRows);
            Assert.Equal(5, subtractive.LoadedNativeRows.Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[0].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[1].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[2].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[3].Count);
            Assert.Equal(20, subtractive.LoadedNativeRows[4].Count);
            Assert.Equal((IEnumerable<byte>)referenceValues[0], (IEnumerable<byte>)subtractive.LoadedNativeRows[0]);
            Assert.Equal((IEnumerable<byte>)referenceValues[1], (IEnumerable<byte>)subtractive.LoadedNativeRows[1]);
            Assert.Equal((IEnumerable<byte>)referenceValues[2], (IEnumerable<byte>)subtractive.LoadedNativeRows[2]);
            Assert.Equal((IEnumerable<byte>)referenceValues[3], (IEnumerable<byte>)subtractive.LoadedNativeRows[3]);
            Assert.Equal((IEnumerable<byte>)referenceValues[4], (IEnumerable<byte>)subtractive.LoadedNativeRows[4]);
        }
    }
}
