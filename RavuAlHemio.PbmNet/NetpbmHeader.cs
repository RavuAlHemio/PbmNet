using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Metadata about a Netpbm image.
    /// </summary>
    public class NetpbmHeader<TPixelComponent>
    {
        private readonly List<Component> _components;

        /// <summary>
        /// The type of this image.
        /// </summary>
        public ImageType ImageType { get; private set; }

        /// <summary>
        /// The width of the image, in pixels.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the image, in pixels.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The number of bytes used per pixel-component.
        /// </summary>
        public int BytesPerPixelComponent { get; private set; }

        /// <summary>
        /// The list of the types of components in this image.
        /// </summary>
        public IList<Component> Components { get { return new ReadOnlyCollection<Component>(_components); } }

        /// <summary>
        /// The largest possible value of a pixel component. (The lowest possible value is always <value>0</value>.)
        /// </summary>
        public TPixelComponent HighestComponentValue { get; private set; }

        /// <summary>
        /// Initializes a new Netpbm image.
        /// </summary>
        /// <param name="imageType">The type of the image.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="bytesPerPixelComponent">Number of bytes used to store a single component of a single
        /// pixel.</param>
        /// <param name="components">The components of the image.</param>
        /// <param name="highestComponentValue">The highest value a pixel component may have.</param>
        public NetpbmHeader(ImageType imageType, int width, int height, int bytesPerPixelComponent,
            IEnumerable<Component> components, TPixelComponent highestComponentValue)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", width, "width must be at least 1");
            }
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", height, "height must be at least 1");
            }
            if (bytesPerPixelComponent < 1)
            {
                throw new ArgumentOutOfRangeException("bytesPerPixelComponent", bytesPerPixelComponent,
                    "bytesPerPixelComponent must be greater than zero");
            }

            _components = new List<Component>(components);
            if (_components.Count == 0)
            {
                throw new ArgumentOutOfRangeException("components", components,
                    "components must contain at least one element");
            }

            ImageType = imageType;
            Width = width;
            Height = height;
            BytesPerPixelComponent = bytesPerPixelComponent;
            HighestComponentValue = highestComponentValue;
        }
    }
}

