using System.Collections.Generic;
using System.Numerics;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with an arbitrary amount of bits per pixel.
    /// </summary>
    public class NetpbmImageBigInteger : NetpbmImage<BigInteger>
    {
        public NetpbmImageBigInteger(NetpbmHeader<BigInteger> header, IEnumerable<IEnumerable<BigInteger>> pixelData)
            : base(header, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(BigInteger component)
        {
            return component.Sign != -1 && component <= Header.HighestComponentValue;
        }

        public override double ScalePixelComponent(BigInteger pixelComponent)
        {
            return (double)pixelComponent / (double)Header.HighestComponentValue;
        }

        internal override IEnumerable<byte> ComponentToBigEndianBytes(BigInteger pixelComponent)
        {
            var list = new List<byte>(pixelComponent.ToByteArray());

            // list is the little-endian representation!

            // strip off leading zeroes
            while (list[list.Count - 1] == 0)
            {
                list.RemoveAt(list.Count - 1);
            }

            // extend to the necessary length
            var requiredLength = Header.BytesPerPixelComponent;
            while (list.Count < requiredLength)
            {
                list.Add(0);
            }

            // reverse list to get big-endian representation
            list.Reverse();

            return list;
        }

        internal override BigInteger InvertPixelValue(BigInteger pixelComponent)
        {
            return Header.HighestComponentValue - pixelComponent;
        }

        internal override bool IsComponentValueZero(BigInteger componentValue)
        {
            return componentValue.IsZero;
        }

        public override NetpbmImage<BigInteger> NewImageOfSameType(NetpbmHeader<BigInteger> header,
            IEnumerable<IEnumerable<BigInteger>> pixelData)
        {
            return new NetpbmImageBigInteger(header, pixelData);
        }
    }
}
