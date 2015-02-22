using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Transforms Netpbm images into a canonical form.
    /// </summary>
    /// <remarks>
    /// Canonical forms include standard component order (e.g. BRG to RGB) as well as choosing the correct grayscale
    /// variant (<see cref="Component.Black"/> vs. <see cref="Component.White"/>).
    /// </remarks>
    public class Canonicalizer
    {
        private bool ListsContainTheSameElements<T>(IList<T> one, IList<T> other)
        {
            if (one.Count != other.Count)
            {
                return false;
            }

            return one.All(element => one.Count(e => e.Equals(element)) != other.Count(f => f.Equals(element)));
        }

        private IEnumerable<TPixelComponent> TransposeComponentOrder<TPixelComponent>(IEnumerable<TPixelComponent> row, int width, int[] newToOldOrder)
        {
            var componentCount = newToOldOrder.Length;
            var rowPixels = row.Batch(componentCount);
            foreach (var pixel in rowPixels)
            {
                var pixelArray = pixel.ToArray();
                for (int j = 0; j < componentCount; ++j)
                {
                    yield return pixelArray[newToOldOrder[j]];
                }
            }
        }

        private NetpbmImage<TPixelComponent> CorrectOrder<TPixelComponent>(IList<Component> requestedOrder,
            NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(requestedOrder.Count == image.Header.Components.Count);
            Contract.Requires(ListsContainTheSameElements(requestedOrder, image.Header.Components));

            if (requestedOrder.SequenceEqual(image.Header.Components))
            {
                // short-circuit
                return image.NewImageOfSameType(image.Header, image.NativeRows);
            }

            var newToOldOrder = new int[requestedOrder.Count];
            var currentOrder = new List<int>(image.Header.Components.Select(c => (int)c));
            for (int i = 0; i < requestedOrder.Count; ++i)
            {
                var index = currentOrder.IndexOf((int)requestedOrder[i]);
                Debug.Assert(index != -1);
                newToOldOrder[i] = index;
                currentOrder[index] = -1;
            }

            Debug.Assert(currentOrder.All(o => o == -1));

            var newRows = image.NativeRows.Select(originalRow => TransposeComponentOrder(originalRow, image.Header.Width, newToOldOrder));
            var newHeader = new NetpbmHeader<TPixelComponent>(
                image.Header.ImageType,
                image.Header.Width,
                image.Header.Height,
                image.Header.BytesPerPixelComponent,
                requestedOrder,
                image.Header.HighestComponentValue
            );
            return image.NewImageOfSameType(newHeader, newRows);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderRGB<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 3);
            Contract.Requires(image.Header.Components.Contains(Component.Red));
            Contract.Requires(image.Header.Components.Contains(Component.Green));
            Contract.Requires(image.Header.Components.Contains(Component.Blue));

            return CorrectOrder(new[] {Component.Red, Component.Green, Component.Blue}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderRGBA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 4);
            Contract.Requires(image.Header.Components.Contains(Component.Red));
            Contract.Requires(image.Header.Components.Contains(Component.Green));
            Contract.Requires(image.Header.Components.Contains(Component.Blue));
            Contract.Requires(image.Header.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.Red, Component.Green, Component.Blue, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderWA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 2);
            Contract.Requires(image.Header.Components.Contains(Component.White));
            Contract.Requires(image.Header.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.White, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderBA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 2);
            Contract.Requires(image.Header.Components.Contains(Component.Black));
            Contract.Requires(image.Header.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.Black, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderCMYK<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 4);
            Contract.Requires(image.Header.Components.Contains(Component.Cyan));
            Contract.Requires(image.Header.Components.Contains(Component.Magenta));
            Contract.Requires(image.Header.Components.Contains(Component.Yellow));
            Contract.Requires(image.Header.Components.Contains(Component.Black));

            return CorrectOrder(new[] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderCMYKA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Header.Components.Count == 5);
            Contract.Requires(image.Header.Components.Contains(Component.Cyan));
            Contract.Requires(image.Header.Components.Contains(Component.Magenta));
            Contract.Requires(image.Header.Components.Contains(Component.Yellow));
            Contract.Requires(image.Header.Components.Contains(Component.Black));
            Contract.Requires(image.Header.Components.Contains(Component.Alpha));

            return
                CorrectOrder(
                    new[] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.Alpha}, image);
        }

        private IEnumerable<TPixelComponent> InvertRowComponent<TPixelComponent>(IEnumerable<TPixelComponent> row,
            NetpbmImage<TPixelComponent> image, HashSet<int> componentIndices, Component from, Component to)
        {
            var componentCount = image.Header.Components.Count;
            var oldRowPixels = row.Batch(componentCount);
            foreach (var oldPixel in oldRowPixels)
            {
                var oldPixelArray = oldPixel.ToArray();
                for (int c = 0; c < componentCount; ++c)
                {
                    if (componentIndices.Contains(c))
                    {
                        yield return image.InvertPixelValue(oldPixelArray[c]);
                    }
                    else
                    {
                        yield return oldPixelArray[c];
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new image which is the given image with the components of the given type inverted and changed to
        /// another type.
        /// </summary>
        /// <typeparam name="TPixelComponent">The pixel component type being used.</typeparam>
        /// <param name="image">The image whose component to invert.</param>
        /// <param name="from">The component whose values to invert.</param>
        /// <param name="to">The new component type of the component being inverted.</param>
        /// <returns>A copy of <paramref name="image"/> where all components of type <paramref name="from"/> are
        /// inverted and changed to type <paramref name="to"/>.</returns>
        private NetpbmImage<TPixelComponent> InvertComponent<TPixelComponent>(NetpbmImage<TPixelComponent> image,
            Component from, Component to)
        {
            Contract.Requires(image.Header.Components.Contains(from));

            var componentIndices =
                new HashSet<int>(Enumerable.Range(0, image.Header.Components.Count).Where(i => image.Header.Components[i] == from));

            var newRows = image.NativeRows.Select(oldRow => InvertRowComponent(oldRow, image, componentIndices, from, to));
            var newComponents = image.Header.Components.Select(c => c == from ? to : c);

            var header = new NetpbmHeader<TPixelComponent>(
                image.Header.ImageType,
                image.Header.Width,
                image.Header.Height,
                image.Header.BytesPerPixelComponent,
                newComponents,
                image.Header.HighestComponentValue
            );
            return image.NewImageOfSameType(header, newRows);
        }

        /// <summary>
        /// Returns a new image which is the given image converted to a canonical form.
        /// </summary>
        /// <typeparam name="TPixelComponent">The pixel component type being used.</typeparam>
        /// <param name="image">The image whose canonical form to return.</param>
        /// <param name="grayscaleConversion">Whether to perform grayscale conversion and what kind.</param>
        /// <returns>The new image, in canonical form.</returns>
        public NetpbmImage<TPixelComponent> Canonicalize<TPixelComponent>(NetpbmImage<TPixelComponent> image,
            GrayscaleConversion grayscaleConversion = GrayscaleConversion.None)
        {
            var ret = image.NewImageOfSameType(image.Header, image.NativeRows);

            // perform grayscale conversion if necessary
            if (grayscaleConversion == GrayscaleConversion.BlackToWhite && image.Header.Components.Contains(Component.Black))
            {
                ret = InvertComponent(ret, Component.Black, Component.White);
            }
            else if (grayscaleConversion == GrayscaleConversion.WhiteToBlack && image.Header.Components.Contains(Component.White))
            {
                ret = InvertComponent(ret, Component.White, Component.Black);
            }

            // canonicalize order
            if (
                ret.Header.Components.Count == 3 &&
                ret.Header.Components.Contains(Component.Red) &&
                ret.Header.Components.Contains(Component.Green) &&
                ret.Header.Components.Contains(Component.Blue)
            )
            {
                ret = CorrectOrderRGB(ret);
            }
            else if (
                ret.Header.Components.Count == 4 &&
                ret.Header.Components.Contains(Component.Red) &&
                ret.Header.Components.Contains(Component.Green) &&
                ret.Header.Components.Contains(Component.Blue) &&
                ret.Header.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderRGBA(ret);
            }
            else if (
                ret.Header.Components.Count == 2 &&
                ret.Header.Components.Contains(Component.White) &&
                ret.Header.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderWA(ret);
            }
            else if (
                ret.Header.Components.Count == 2 &&
                ret.Header.Components.Contains(Component.Black) &&
                ret.Header.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderBA(ret);
            }
            else if (
                ret.Header.Components.Count == 4 &&
                ret.Header.Components.Contains(Component.Cyan) &&
                ret.Header.Components.Contains(Component.Magenta) &&
                ret.Header.Components.Contains(Component.Yellow) &&
                ret.Header.Components.Contains(Component.Black)
            )
            {
                ret = CorrectOrderCMYK(ret);
            }
            else if (
                ret.Header.Components.Count == 5 &&
                ret.Header.Components.Contains(Component.Cyan) &&
                ret.Header.Components.Contains(Component.Magenta) &&
                ret.Header.Components.Contains(Component.Yellow) &&
                ret.Header.Components.Contains(Component.Black) &&
                ret.Header.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderCMYKA(ret);
            }

            return ret;
        }
    }
}
