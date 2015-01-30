using System.Collections.Generic;
using System.Numerics;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with an arbitrary amount of bits per pixel.
    /// </summary>
    public class PamPgmPpmImageBigInteger : PamPgmPpmImage<BigInteger>
    {
        public PamPgmPpmImageBigInteger(int width, int height, BigInteger highestComponentValue,
            IEnumerable<Component> components, IEnumerable<IEnumerable<IEnumerable<BigInteger>>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return HighestComponentValue.ToByteArray().Length; }
        }

        protected override double ScalePixelComponent(BigInteger pixelComponent)
        {
            return (double)pixelComponent / (double)HighestComponentValue;
        }
    }
}
