using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image.
    /// </summary>
    public abstract class NetpbmImage<TPixelComponent>
    {
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
        public abstract int BytesPerPixelComponent { get; }

		/// <summary>
		/// The list of the types of components in this image.
		/// </summary>
        public IList<Component> Components { get; private set; }

        /// <summary>
        /// The actual rows of the image; drill down from rows to pixels. Each pixel is represented as a sequence of
		/// components within the row; therefore, each row has <see cref="Width"/> times <see cref="Components.Count"/>
		/// elements.
        /// </summary>
        protected IList<IList<TPixelComponent>> Rows { get; set; }

        /// <summary>
        /// The largest possible value of a pixel component. (The lowest possible value is always <value>0</value>.)
        /// </summary>
        public TPixelComponent HighestComponentValue { get; protected set; }

        /// <summary>
        /// Stores whether this image is a bitmap (only allows two values per pixel component).
        /// </summary>
        /// <value><c>true</c> if this image is a bitmap; otherwise, <c>false</c>.</value>
        public abstract bool IsBitmap { get; }

        /// <summary>
        /// Scales a pixel into the interval [<value>0.0</value>, <value>1.0</value>].
        /// </summary>
        public abstract double ScalePixelComponent(TPixelComponent pixelComponent);

        /// <summary>
        /// Inverts the given pixel value (subtracts it from <see cref="HighestComponentValue"/>).
        /// </summary>
        /// <returns>The inverted pixel value.</returns>
        /// <param name="pixelComponent">The value to invert.</param>
        internal abstract TPixelComponent InvertPixelValue(TPixelComponent pixelComponent);

        /// <summary>
        /// Converts a pixel component value into a sequence of bytes, most significant byte first (big-endian format).
        /// </summary>
        /// <returns>The sequence of bytes representing the pixel component value.</returns>
        /// <param name="pixelComponent">The pixel component value to encode.</param>
        internal abstract IEnumerable<byte> ComponentToBigEndianBytes(TPixelComponent pixelComponent);

        /// <summary>
        /// Returns whether the given pixel component value is the zero value.
        /// </summary>
        /// <returns><c>true</c> if the given pixel component value is the zero value; <c>false</c> otherwise.</returns>
        /// <param name="componentValue">The component whose value to check for zero.</param>
        internal abstract bool IsComponentValueZero(TPixelComponent componentValue);

        /// <summary>
        /// Obtains the actual values of the pixel at the given coordinates, as a list of components (in the order
        /// given by <see cref="Components"/>). Each value lies within the interval [<value>0</value>,
        /// <see cref="HighestComponentValue"/>].
        /// </summary>
        /// <param name="x">The zero-based X coordinate of the pixel to fetch.</param>
        /// <param name="y">The zero-based Y coordinate of the pixel to fetch.</param>
        /// <returns>A list of pixel values within the interval [<value>0</value>,
        /// <see cref="HighestComponentValue"/>].</returns>
        public IList<TPixelComponent> GetNativePixel(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException("x", x,
                    string.Format("x ({0}) must be at least 0 and less than the width of the image ({1})", x, Width));
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("y", y,
                    string.Format("y ({0}) must be at least 0 and less than the height of the image ({1})", y, Height));
            }
            var retArray = Rows[y].Skip(x * Components.Count).Take(Components.Count).ToArray();
            var ret = new ReadOnlyCollection<TPixelComponent>(retArray);
            Contract.Ensures(ret.Count == Components.Count);
            Contract.Ensures(ret.All(IsPixelComponentInRange));
            return ret;
        }

        /// <summary>
        /// Obtains the values of the pixel at the given coordinates, as a list of components (in the order
        /// given by <see cref="Components"/>). Each value is scaled to lie within the interval [<value>0.0</value>,
        /// <value>1.0</value>].
        /// </summary>
        /// <param name="x">The zero-based X coordinate of the pixel to return.</param>
        /// <param name="y">The zero-based Y coordinate of the pixel to return.</param>
        /// <returns>A list of pixel values within the interval [<value>0.0</value>, <value>1.0</value>].</returns>
        public IList<double> GetScaledPixel(int x, int y)
        {
            var retArray = GetNativePixel(x, y).Select(p => ScalePixelComponent(p)).ToArray();
            var ret = new ReadOnlyCollection<double>(retArray);
            Contract.Ensures(ret.Count == Components.Count);
            Contract.Ensures(ret.All(component => component >= 0.0 && component <= 1.0));
            return ret;
        }

        /// <summary>
        /// Obtains the pixel values in the given row, as a contiguous list of component values for each pixel (in
        /// pixel-major order, e.g. <c>RGBRGBRGB</c>).
        /// </summary>
        /// <returns>The list of pixels' component values within the given row.</returns>
        /// <param name="y">The zero-based coordinate of the row whose pixels to return.</param>
        public IList<TPixelComponent> GetNativeRow(int y)
        {
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("y", y,
                    string.Format("y ({0}) must be at least 0 and less than the height of the image ({1})", y, Height));
            }
            return new ReadOnlyCollection<TPixelComponent>(Rows[y]);
        }

        /// <summary>
        /// Returns whether the pixel component is in the allowed range (i.e. in the interval [<value>0</value>,
        /// <see cref="HighestComponentValue"/>].
        /// </summary>
        /// <returns><c>true</c> if the pixel component is in range; otherwise, <c>false</c>.</returns>
        /// <param name="component">The pixel component to test.</param>
        protected abstract bool IsPixelComponentInRange(TPixelComponent component);

        /// <summary>
        /// Initializes a new Netpbm image.
        /// </summary>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="highestComponentValue">The highest value a pixel component may have.</param>
        /// <param name="components">The components of the image.</param>
        /// <param name="pixelData">The actual pixel data of the image. Drill down rows to pixels, with the pixels
        /// represented as a contiguous sequence of component values for each pixel (in pixel-major order, e.g.
        /// <c>RGBRGBRGB</c>.</param>
        protected NetpbmImage(int width, int height, TPixelComponent highestComponentValue,
			IEnumerable<Component> components, IEnumerable<IEnumerable<TPixelComponent>> pixelData)
        {
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", width, "width must be at least 1");
            }
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", height, "height must be at least 1");
            }

            Width = width;
            Height = height;
            HighestComponentValue = highestComponentValue;
            Components = new List<Component>(components);

            Rows = new List<IList<TPixelComponent>>(Height);
            foreach (var row in pixelData)
            {
				var rowPixels = new List<TPixelComponent>(row);
				if (rowPixels.Count != Width * Components.Count)
				{
                    throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("row {0} must have {1} values ({2} pixels times {3} components) but only has {4}",
                            Rows.Count, Width * Components.Count, Width, Components.Count, rowPixels.Count));
				}

                var badPixel = rowPixels.FindIndex(c => !IsPixelComponentInRange(c));
                if (badPixel != -1)
                {
                    throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("row {0} pixel {1} component {2} must have a value between 0 and {3} (has {4})",
                            Rows.Count, badPixel / Components.Count, badPixel % Components.Count, HighestComponentValue,
                            rowPixels[badPixel]));
                }

                Rows.Add(rowPixels);
                if (Rows.Count > Height)
                {
                    throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("image has at least {0} rows, must have {1}", Rows.Count, height));
                }
            }
            if (Rows.Count != Height)
            {
                throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("image has {0} rows, must have {1}", Rows.Count, height));
            }
        }

        [ContractInvariantMethod]
        protected void ObjectInvariant()
        {
            Contract.Invariant(Width > 0);
            Contract.Invariant(Height > 0);
            Contract.Invariant(BytesPerPixelComponent > 0);
            Contract.Invariant(Rows.Count == Height);
            Contract.Invariant(Rows.All(row => row.Count == Width * Components.Count));
            Contract.Invariant(Rows.All(row => row.All(IsPixelComponentInRange)));
        }
    }
}
