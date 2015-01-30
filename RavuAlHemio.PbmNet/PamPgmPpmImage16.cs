using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with 8 bits per pixel.
    /// </summary>
    public class PamPgmPpmImage16 : PamPgmPpmImage<ushort>
    {
        public PamPgmPpmImage16(int width, int height, ushort highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<IEnumerable<ushort>>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return 2; }
        }

        protected override double ScalePixelComponent(ushort pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }
    }
}
