using System.Collections.Generic;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image with 32 bits per pixel component.
    /// </summary>
    public class NetpbmImage32 : NetpbmImage<uint>
    {
        public NetpbmImage32(NetpbmHeader<uint> header, IEnumerable<IEnumerable<uint>> pixelData)
            : base(header, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(uint component)
        {
            return component <= Header.HighestComponentValue;
        }

        public override double ScalePixelComponent(uint pixelComponent)
        {
            return pixelComponent / (double)Header.HighestComponentValue;
        }

        internal override IEnumerable<byte> ComponentToBigEndianBytes(uint pixelComponent)
        {
            yield return (byte)((pixelComponent >> 24) & 0xFF);
            yield return (byte)((pixelComponent >> 16) & 0xFF);
            yield return (byte)((pixelComponent >>  8) & 0xFF);
            yield return (byte)((pixelComponent >>  0) & 0xFF);
        }

        internal override uint InvertPixelValue(uint pixelComponent)
        {
            return Header.HighestComponentValue - pixelComponent;
        }

        internal override bool IsComponentValueZero(uint componentValue)
        {
            return (componentValue == 0);
        }

        public override NetpbmImage<uint> NewImageOfSameType(NetpbmHeader<uint> header,
            IEnumerable<IEnumerable<uint>> pixelData)
        {
            return new NetpbmImage32(header, pixelData);
        }
    }
}
