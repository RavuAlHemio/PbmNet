using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with 8 bits per pixel.
    /// </summary>
    public class PamPgmPpmImage8 : PamPgmPpmImage<byte>
    {
        public PamPgmPpmImage8(int width, int height, byte highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<IEnumerable<byte>>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return 1; }
        }

        protected override double ScalePixelComponent(byte pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }
    }
}
