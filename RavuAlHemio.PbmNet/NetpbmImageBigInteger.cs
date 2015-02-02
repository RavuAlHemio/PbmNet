using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image with an arbitrary amount of bits per pixel.
    /// </summary>
    public class NetpbmImageBigInteger : NetpbmImage<BigInteger>
    {
        public NetpbmImageBigInteger(int width, int height, BigInteger highestComponentValue,
            IEnumerable<Component> components, IEnumerable<IEnumerable<BigInteger>> pixelData)
            : base(width, height, highestComponentValue, components, pixelData)
        {
        }

        protected override bool IsPixelComponentInRange(BigInteger component)
        {
            return component.Sign != -1 && component <= HighestComponentValue;
        }

        public override int BytesPerPixelComponent
        {
            get { return ComponentToBigEndianBytes(HighestComponentValue).Count(); }
        }

        public override double ScalePixelComponent(BigInteger pixelComponent)
        {
            return (double)pixelComponent / (double)HighestComponentValue;
        }

        public override bool IsBitmap
        {
            get
            {
                return HighestComponentValue.IsOne;
            }
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
            var requiredLength = BytesPerPixelComponent;
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
            return HighestComponentValue - pixelComponent;
        }

        internal override bool IsComponentValueZero(BigInteger componentValue)
        {
            return componentValue.IsZero;
        }

        public override NetpbmImage<BigInteger> NewImageOfSameType(int width, int height, BigInteger highestComponentValue, IEnumerable<Component> components,
            IEnumerable<IEnumerable<BigInteger>> pixelData)
        {
            return new NetpbmImageBigInteger(width, height, highestComponentValue, components, pixelData);
        }
    }
}
