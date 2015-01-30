using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A PAM, PGM or PPM image.
    /// </summary>
    public abstract class PamPgmPpmImage<TPixelComponent> : INetPbmImage
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public abstract int BytesPerPixelComponent { get; }

        public IList<Component> Components { get; private set; }

        /// <summary>
        /// The actual rows of the image; drill down rows to pixels to components.
        /// </summary>
        private IList<IList<IList<TPixelComponent>>> Rows { get; set; }

        /// <summary>
        /// The largest possible value of a pixel component. (The lowest possible value is always <value>0</value>.)
        /// </summary>
        public TPixelComponent HighestComponentValue { get; protected set; }

        /// <summary>
        /// Scales a pixel into the interval [<value>0.0</value>, <value>1.0</value>].
        /// </summary>
        protected abstract double ScalePixelComponent(TPixelComponent pixelComponent);

        /// <summary>
        /// Obtains the actual values of the pixel at the given coordinates, component by component (in the order given
        /// by <see cref="Components"/>). Each value lies within the interval [<value>0</value>,
        /// <see cref="HighestComponentValue"/>].
        /// </summary>
        /// <param name="x">The X coordinate of the pixel to fetch.</param>
        /// <param name="y">The Y coordinate of the pixel to fetch.</param>
        /// <returns>A list of pixel values scaled to the interval [<value>0</value>,
        /// <see cref="HighestComponentValue"/>].</returns>
        public IList<TPixelComponent> GetNativePixel(int x, int y)
        {
            return new ReadOnlyCollection<TPixelComponent>(Rows[y][x]);
        }

        public IList<double> GetScaledPixel(int x, int y)
        {
            var ret = new double[Components.Count];
            for (int i = 0; i < Components.Count; ++i)
            {
                ret[i] = ScalePixelComponent(Rows[x][y][i]);
            }
            return new ReadOnlyCollection<double>(ret);
        }

        /// <summary>
        /// Initializes a new 8-bit-per-component PGM or PPM image.
        /// </summary>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="highestComponentValue">The highest value a pixel component may have.</param>
        /// <param name="components">The components of the image.</param>
        /// <param name="pixelData">The actual pixel data of the image, drill down rows to pixels to components.</param>
        protected PamPgmPpmImage(int width, int height, TPixelComponent highestComponentValue,
            IEnumerable<Component> components, IEnumerable<IEnumerable<IEnumerable<TPixelComponent>>> pixelData)
        {
            Width = width;
            Height = height;
            HighestComponentValue = highestComponentValue;
            Components = new List<Component>(components);

            Rows = new List<IList<IList<TPixelComponent>>>(Height);
            foreach (var row in pixelData)
            {
                var rowPixels = new List<IList<TPixelComponent>>(Width);
                foreach (var pixel in row)
                {
                    var pixelComponents = new List<TPixelComponent>(pixel);
                    if (pixelComponents.Count != Components.Count)
                    {
                        throw new ArgumentOutOfRangeException("pixelData",
                            string.Format("row {0} column {1}: pixel has {2} components, must have {3}", Rows.Count,
                                rowPixels.Count, pixelComponents.Count, Components.Count));
                    }
                    rowPixels.Add(pixelComponents);

                    if (rowPixels.Count > Width)
                    {
                        throw new ArgumentOutOfRangeException("pixelData",
                            string.Format("row {0}: row has {1} pixels, must have {2}", Rows.Count, rowPixels.Count, Width));
                    }
                }
                if (rowPixels.Count != Width)
                {
                    throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("row {0}: row has {1} pixels, must have {2}", Rows.Count, rowPixels.Count, Width));
                }

                Rows.Add(rowPixels);
                if (Rows.Count > Height)
                {
                    throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("image has {0} rows, must have {1}", Rows.Count, height));
                }
            }
            if (Rows.Count != Height)
            {
                throw new ArgumentOutOfRangeException("pixelData",
                        string.Format("image has {0} rows, must have {1}", Rows.Count, height));
            }
        }
    }
}
