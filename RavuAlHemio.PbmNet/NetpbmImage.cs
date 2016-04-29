using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// An in-memory representation of a Netpbm image.
    /// </summary>
    public abstract class NetpbmImage<TPixelComponent>
    {
        private bool _loaded;

        /// <summary>
        /// The header containing the image's metadata.
        /// </summary>
        public NetpbmHeader<TPixelComponent> Header { get; private set; }

        /// <summary>
        /// The actual rows of the image; drill down from rows to pixels. Each pixel is represented as a sequence of
		/// components within the row; therefore, each row has <see cref="NetpbmHeader{TPixelComponent}.Width"/> times
		/// <see cref="NetpbmHeader{TPixelComponent}.Components"/>.Count elements.
        /// </summary>
        protected IEnumerable<IEnumerable<TPixelComponent>> Rows { get; set; }

        /// <summary>
        /// Scales a pixel into the interval [<c>0.0</c>, <c>1.0</c>].
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
        /// given by <see cref="NetpbmHeader{TPixelComponent}.Components"/>). Each value lies within the interval
        /// [<c>0</c>, <see cref="NetpbmHeader{TPixelComponent}.HighestComponentValue"/>].
        /// </summary>
        /// <param name="x">The zero-based X coordinate of the pixel to fetch.</param>
        /// <param name="y">The zero-based Y coordinate of the pixel to fetch.</param>
        /// <returns>A list of pixel values within the interval [<c>0</c>,
        /// <see cref="NetpbmHeader{TPixelComponent}.HighestComponentValue"/>].</returns>
        public IList<TPixelComponent> GetNativePixel(int x, int y)
        {
            if (x < 0 || x >= Header.Width)
            {
                throw new ArgumentOutOfRangeException("x", x,
                    string.Format("x ({0}) must be at least 0 and less than the width of the image ({1})", x, Header.Width));
            }
            if (y < 0 || y >= Header.Height)
            {
                throw new ArgumentOutOfRangeException("y", y,
                    string.Format("y ({0}) must be at least 0 and less than the height of the image ({1})", y, Header.Height));
            }
            var retArray = Rows.Skip(y).First().Skip(x * Header.Components.Count).Take(Header.Components.Count).ToArray();
            var ret = new ReadOnlyCollection<TPixelComponent>(retArray);
            Contract.Ensures(ret.Count == Header.Components.Count);
            Contract.Ensures(ret.All(IsPixelComponentInRange));
            return ret;
        }

        /// <summary>
        /// Obtains the values of the pixel at the given coordinates, as a list of components (in the order
        /// given by <see cref="NetpbmHeader{TPixelComponent}.Components"/>). Each value is scaled to lie within the
        /// interval [<c>0.0</c>, <c>1.0</c>].
        /// </summary>
        /// <param name="x">The zero-based X coordinate of the pixel to return.</param>
        /// <param name="y">The zero-based Y coordinate of the pixel to return.</param>
        /// <returns>A list of pixel values within the interval [<c>0.0</c>, <c>1.0</c>].</returns>
        public IList<double> GetScaledPixel(int x, int y)
        {
            var retArray = GetNativePixel(x, y).Select(ScalePixelComponent).ToArray();
            var ret = new ReadOnlyCollection<double>(retArray);
            Contract.Ensures(ret.Count == Header.Components.Count);
            Contract.Ensures(ret.All(component => component >= 0.0 && component <= 1.0));
            return ret;
        }

        /// <summary>
        /// Obtains an enumerable of rows, with their pixel values as a contiguous enumerable of component values for
        /// each pixel in pixel-major order, e.g. <c>RGBRGBRGB</c>.
        /// </summary>
        /// <value>The list of rows.</value>
        public IEnumerable<IEnumerable<TPixelComponent>> NativeRows
        {
            get
            {
                var retRows = Rows.Select(r => new ReadOnlyEnumerableShim<TPixelComponent>(r));
                return new ReadOnlyEnumerableShim<IEnumerable<TPixelComponent>>(retRows);
            }
        }

        /// <summary>
        /// If the image has been loaded using <see cref="LoadData"/>, obtains a list of rows, with their pixel values
        /// as a contiguous list of component values for each pixel in pixel-major order, e.g. <c>RGBRGBRGB</c>. If the
        /// image has not been loaded, returns <c>null</c>.
        /// </summary>
        /// <value>The list of rows, or <c>null</c>.</value>
        public IList<IList<TPixelComponent>> LoadedNativeRows
        {
            get
            {
                if (!_loaded)
                {
                    return null;
                }

                var retRows = Rows.Select(r => (IList<TPixelComponent>)new ReadOnlyCollection<TPixelComponent>((IList<TPixelComponent>)r)).ToList();
                return new ReadOnlyCollection<IList<TPixelComponent>>(retRows);
            }
        }

        /// <summary>
        /// Returns whether the pixel component is in the allowed range (i.e. in the interval [<c>0</c>,
        /// <see cref="NetpbmHeader{TPixelComponent}.HighestComponentValue"/>].
        /// </summary>
        /// <returns><c>true</c> if the pixel component is in range; otherwise, <c>false</c>.</returns>
        /// <param name="component">The pixel component to test.</param>
        protected abstract bool IsPixelComponentInRange(TPixelComponent component);

        /// <summary>
        /// Creates and returns a new image of the same type as this image.
        /// </summary>
        /// <param name="header">The header of the new image.</param>
        /// <param name="pixelData">The actual pixel data of the image. Drill down rows to pixels, with the pixels
        /// represented as a contiguous sequence of component values for each pixel (in pixel-major order, e.g.
        /// <c>RGBRGBRGB</c>.</param>
        /// <returns>The new image.</returns>
        public abstract NetpbmImage<TPixelComponent> NewImageOfSameType(NetpbmHeader<TPixelComponent> header,
            IEnumerable<IEnumerable<TPixelComponent>> pixelData);

        /// <summary>
        /// Initializes a new Netpbm image.
        /// </summary>
        /// <param name="header">The header of the image, containing all necessary information.</param>
        /// <param name="pixelData">The actual pixel data of the image. Drill down rows to pixels, with the pixels
        /// represented as a contiguous sequence of component values for each pixel (in pixel-major order, e.g.
        /// <c>RGBRGBRGB</c>. The data passed to this constructor must not be modified after the constructor
        /// completes, unless <see cref="LoadData"/> is called.</param>
        protected NetpbmImage(NetpbmHeader<TPixelComponent> header, IEnumerable<IEnumerable<TPixelComponent>> pixelData)
        {
            Header = header;
            Rows = pixelData;
            _loaded = false;
        }

        /// <summary>
        /// Performs an immediate load of the image data, creating a copy of the pixel data passed to the constructor.
        /// </summary>
        public void LoadData()
        {
            // collapses the IEnumerable<IEnumerable> into a List<List>
            var myRows = new List<IList<TPixelComponent>>(Header.Height);

            // pre-allocate the lists
            for (int i = 0; i < Header.Height; ++i)
            {
                myRows.Add(new List<TPixelComponent>(Header.Width*Header.Components.Count));
            }

            int rowI = -1;
            foreach (var row in Rows)
            {
                ++rowI;
                if (rowI >= Header.Height)
                {
                    throw new InvalidDataException(
                        string.Format("image has at least {0} rows, must have {1}", rowI + 1, Header.Height));
                }

                List<TPixelComponent> rowPixels = (List<TPixelComponent>)myRows[rowI];
                try
                {
                    rowPixels.AddRange(row);
                }
                catch (OverflowException exc)
                {
                    throw new InvalidDataException(string.Format("row {0} contains a pixel value that is way out of range", myRows.Count),
                        exc);
                }
                
                if (rowPixels.Count != Header.Width * Header.Components.Count)
                {
                    throw new InvalidDataException(
                        string.Format("row {0} must have {1} values ({2} pixels times {3} components) but only has {4}",
                            myRows.Count, Header.Width * Header.Components.Count, Header.Width, Header.Components.Count, rowPixels.Count));
                }

                var badPixel = rowPixels.FindIndex(c => !IsPixelComponentInRange(c));
                if (badPixel != -1)
                {
                    throw new InvalidDataException(
                        string.Format("row {0} pixel {1} component {2} must have a value between 0 and {3} (has {4})",
                            myRows.Count, badPixel / Header.Components.Count, badPixel % Header.Components.Count,
                            Header.HighestComponentValue, rowPixels[badPixel]));
                }
            }
            if (rowI != Header.Height - 1)
            {
                throw new InvalidDataException(
                    string.Format("image has {0} rows, must have {1}", myRows.Count, Header.Height));
            }
            Rows = myRows;
            _loaded = true;
        }

        [ContractInvariantMethod]
        protected void ObjectInvariant()
        {
            Contract.Invariant(Header.Width > 0);
            Contract.Invariant(Header.Height > 0);
            Contract.Invariant(Header.BytesPerPixelComponent > 0);
            Contract.Invariant(Rows.Count() == Header.Height);
            Contract.Invariant(Rows.All(row => row.Count() == Header.Width * Header.Components.Count));
            Contract.Invariant(Rows.All(row => row.All(IsPixelComponentInRange)));
        }
    }
}
