using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 32 bits per pixel component.
    /// </summary>
    public class NetpbmImage32 : NetpbmImage<uint>
    {
        public NetpbmImage32(int width, int height, uint highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<uint>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return 4; }
        }

        public override double ScalePixelComponent(uint pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }
    }
}
