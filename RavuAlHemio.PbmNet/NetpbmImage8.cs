using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 8 bits per pixel component. This is probably the most common
    /// type of image you will encounter.
    /// </summary>
    public class NetpbmImage8 : NetpbmImage<byte>
    {
        public NetpbmImage8(int width, int height, byte highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<byte>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(byte component)
        {
            return component <= HighestComponentValue;
        }

        public override int BytesPerPixelComponent
        {
            get { return 1; }
        }

        public override double ScalePixelComponent(byte pixelComponent)
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

        internal override IEnumerable<byte> ComponentToBigEndianBytes(byte pixelComponent)
        {
            yield return pixelComponent;
        }

        internal override byte InvertPixelValue(byte pixelComponent)
        {
            return (byte)(HighestComponentValue - pixelComponent);
        }

        internal override bool IsComponentValueZero(byte componentValue)
        {
            return (componentValue == 0);
        }

        public override NetpbmImage<byte> NewImageOfSameType(int width, int height, byte highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<byte>> pixelData)
        {
            return new NetpbmImage8(width, height, highestComponentValue, components, pixelData);
        }
    }
}
