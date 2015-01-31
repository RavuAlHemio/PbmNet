using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 16 bits per pixel component.
    /// </summary>
    public class NetpbmImage16 : NetpbmImage<ushort>
    {
        public NetpbmImage16(int width, int height, ushort highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<ushort>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        public override int BytesPerPixelComponent
        {
            get { return 2; }
        }

        public override double ScalePixelComponent(ushort pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }
    }
}
