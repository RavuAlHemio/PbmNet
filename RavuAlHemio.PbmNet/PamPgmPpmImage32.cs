using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with 32 bits per pixel.
    /// </summary>
    public class PamPgmPpmImage32 : PamPgmPpmImage<uint>
    {
        public PamPgmPpmImage32(int width, int height, uint highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<IEnumerable<uint>>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return 4; }
        }

        protected override double ScalePixelComponent(uint pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }
    }
}
