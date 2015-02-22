using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 8 bits per pixel component. This is probably the most common
    /// type of image you will encounter.
    /// </summary>
    public class NetpbmImage8 : NetpbmImage<byte>
    {
        public NetpbmImage8(NetpbmHeader<byte> header, IEnumerable<IEnumerable<byte>> pixelData)
            : base(header, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(byte component)
        {
            return component <= Header.HighestComponentValue;
        }

        public override double ScalePixelComponent(byte pixelComponent)
        {
            return pixelComponent / (double)Header.HighestComponentValue;
        }

        internal override IEnumerable<byte> ComponentToBigEndianBytes(byte pixelComponent)
        {
            yield return pixelComponent;
        }

        internal override byte InvertPixelValue(byte pixelComponent)
        {
            return (byte)(Header.HighestComponentValue - pixelComponent);
        }

        internal override bool IsComponentValueZero(byte componentValue)
        {
            return (componentValue == 0);
        }

        public override NetpbmImage<byte> NewImageOfSameType(NetpbmHeader<byte> header,
            IEnumerable<IEnumerable<byte>> pixelData)
        {
            return new NetpbmImage8(header, pixelData);
        }
    }
}
