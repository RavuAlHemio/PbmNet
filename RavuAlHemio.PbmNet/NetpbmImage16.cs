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

        protected override bool IsPixelComponentInRange(ushort component)
        {
            return component >= 0 && component <= HighestComponentValue;
        }

        public override int BytesPerPixelComponent
        {
            get { return 2; }
        }

        public override double ScalePixelComponent(ushort pixelComponent)
        {
            return pixelComponent / (double)HighestComponentValue;
        }

        public override bool IsBitmap
        {
            get
            {
                return HighestComponentValue == 1;
            }
        }

        internal override IEnumerable<byte> ComponentToBigEndianBytes(ushort pixelComponent)
        {
            yield return (byte)((pixelComponent >> 8) & 0xFF);
            yield return (byte)((pixelComponent >> 0) & 0xFF);
        }

        internal override ushort InvertPixelValue(ushort pixelComponent)
        {
            return (ushort)(HighestComponentValue - pixelComponent);
        }

        internal override bool IsComponentValueZero(ushort componentValue)
        {
            return (componentValue == 0);
        }
    }
}
