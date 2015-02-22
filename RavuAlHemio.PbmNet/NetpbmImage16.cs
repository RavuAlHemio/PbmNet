using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 16 bits per pixel component.
    /// </summary>
    public class NetpbmImage16 : NetpbmImage<ushort>
    {
        public NetpbmImage16(NetpbmHeader<ushort> header, IEnumerable<IEnumerable<ushort>> pixelData)
            : base(header, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(ushort component)
        {
            return component <= Header.HighestComponentValue;
        }

        public override double ScalePixelComponent(ushort pixelComponent)
        {
            return pixelComponent / (double)Header.HighestComponentValue;
        }

        internal override IEnumerable<byte> ComponentToBigEndianBytes(ushort pixelComponent)
        {
            yield return (byte)((pixelComponent >> 8) & 0xFF);
            yield return (byte)((pixelComponent >> 0) & 0xFF);
        }

        internal override ushort InvertPixelValue(ushort pixelComponent)
        {
            return (ushort)(Header.HighestComponentValue - pixelComponent);
        }

        internal override bool IsComponentValueZero(ushort componentValue)
        {
            return (componentValue == 0);
        }

        public override NetpbmImage<ushort> NewImageOfSameType(NetpbmHeader<ushort> header,
            IEnumerable<IEnumerable<ushort>> pixelData)
        {
            return new NetpbmImage16(header, pixelData);
        }
    }
}
