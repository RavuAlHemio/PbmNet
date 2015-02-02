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

        protected override bool IsPixelComponentInRange(uint component)
        {
            return component <= HighestComponentValue;
        }

        public override int BytesPerPixelComponent
        {
            get { return 4; }
        }

        public override double ScalePixelComponent(uint pixelComponent)
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

        internal override IEnumerable<byte> ComponentToBigEndianBytes(uint pixelComponent)
        {
            yield return (byte)((pixelComponent >> 24) & 0xFF);
            yield return (byte)((pixelComponent >> 16) & 0xFF);
            yield return (byte)((pixelComponent >>  8) & 0xFF);
            yield return (byte)((pixelComponent >>  0) & 0xFF);
        }

        internal override uint InvertPixelValue(uint pixelComponent)
        {
            return HighestComponentValue - pixelComponent;
        }

        internal override bool IsComponentValueZero(uint componentValue)
        {
            return (componentValue == 0);
        }

        public override NetpbmImage<uint> NewImageOfSameType(int width, int height, uint highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<uint>> pixelData)
        {
            return new NetpbmImage32(width, height, highestComponentValue, components, pixelData);
        }
    }
}
