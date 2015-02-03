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

        private NetpbmImage<TPixelComponent> CorrectOrder<TPixelComponent>(IList<Component> requestedOrder,
            NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(requestedOrder.Count == image.Components.Count);
            Contract.Requires(ListsContainTheSameElements(requestedOrder, image.Components));

            if (requestedOrder.SequenceEqual(image.Components))
            {
                // short-circuit
                return image.NewImageOfSameType(image.Width, image.Height, image.HighestComponentValue, requestedOrder, image.NativeRows);
            }

            var newToOldOrder = new int[requestedOrder.Count];
            var currentOrder = new List<int>(image.Components.Select(c => (int)c));
            for (int i = 0; i < requestedOrder.Count; ++i)
            {
                var index = currentOrder.IndexOf((int)requestedOrder[i]);
                Debug.Assert(index != -1);
                newToOldOrder[i] = index;
                currentOrder[index] = -1;
            }

            Debug.Assert(currentOrder.All(o => o == -1));

            var rows = new List<TPixelComponent>[image.Height];
            for (int y = 0; y < image.Height; ++y)
            {
                var originalRow = image.NativeRows[y];
                var newRow = new List<TPixelComponent>(image.Width * image.Components.Count);

                for (int x = 0; x < image.Width; ++x)
                {
                    var newChunk = new TPixelComponent[image.Components.Count];
                    for (int c = 0; c < image.Components.Count; ++c)
                    {
                        newChunk[c] = originalRow[x*image.Components.Count + newToOldOrder[c]];
                    }
                    newRow.AddRange(newChunk);
                }

                rows[y] = newRow;
            }

            return image.NewImageOfSameType(image.Width, image.Height, image.HighestComponentValue, requestedOrder, rows);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderRGB<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 3);
            Contract.Requires(image.Components.Contains(Component.Red));
            Contract.Requires(image.Components.Contains(Component.Green));
            Contract.Requires(image.Components.Contains(Component.Blue));

            return CorrectOrder(new[] {Component.Red, Component.Green, Component.Blue}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderRGBA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 4);
            Contract.Requires(image.Components.Contains(Component.Red));
            Contract.Requires(image.Components.Contains(Component.Green));
            Contract.Requires(image.Components.Contains(Component.Blue));
            Contract.Requires(image.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.Red, Component.Green, Component.Blue, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderWA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 2);
            Contract.Requires(image.Components.Contains(Component.White));
            Contract.Requires(image.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.White, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderBA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 2);
            Contract.Requires(image.Components.Contains(Component.Black));
            Contract.Requires(image.Components.Contains(Component.Alpha));

            return CorrectOrder(new[] {Component.Black, Component.Alpha}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderCMYK<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 4);
            Contract.Requires(image.Components.Contains(Component.Cyan));
            Contract.Requires(image.Components.Contains(Component.Magenta));
            Contract.Requires(image.Components.Contains(Component.Yellow));
            Contract.Requires(image.Components.Contains(Component.Black));

            return CorrectOrder(new[] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black}, image);
        }

        private NetpbmImage<TPixelComponent> CorrectOrderCMYKA<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            Contract.Requires(image.Components.Count == 5);
            Contract.Requires(image.Components.Contains(Component.Cyan));
            Contract.Requires(image.Components.Contains(Component.Magenta));
            Contract.Requires(image.Components.Contains(Component.Yellow));
            Contract.Requires(image.Components.Contains(Component.Black));
            Contract.Requires(image.Components.Contains(Component.Alpha));

            return
                CorrectOrder(
                    new[] {Component.Cyan, Component.Magenta, Component.Yellow, Component.Black, Component.Alpha}, image);
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
            Contract.Requires(image.Components.Contains(from));

            var componentIndices =
                new HashSet<int>(Enumerable.Range(0, image.Components.Count).Where(i => image.Components[i] == from));
            var newRows = new List<TPixelComponent>[image.Height];

            for (int y = 0; y < image.Height; ++y)
            {
                var oldRow = image.NativeRows[y];
                var newRow = new List<TPixelComponent>(image.Width*image.Components.Count);
                for (int x = 0; x < image.Width; ++x)
                {
                    for (int c = 0; c < image.Components.Count; ++c)
                    {
                        var value = oldRow[x*image.Components.Count + c];
                        if (componentIndices.Contains(c))
                        {
                            value = image.InvertPixelValue(value);
                        }
                        newRow.Add(value);
                    }
                }
                newRows[y] = newRow;
            }

            var newComponents = image.Components.Select(c => c == from ? to : c);

            return image.NewImageOfSameType(image.Width, image.Height, image.HighestComponentValue, newComponents, newRows);
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
            var ret = image.NewImageOfSameType(image.Width, image.Height, image.HighestComponentValue, image.Components, image.NativeRows);

            // perform grayscale conversion if necessary
            if (grayscaleConversion == GrayscaleConversion.BlackToWhite && image.Components.Contains(Component.Black))
            {
                ret = InvertComponent(ret, Component.Black, Component.White);
            }
            else if (grayscaleConversion == GrayscaleConversion.WhiteToBlack && image.Components.Contains(Component.White))
            {
                ret = InvertComponent(ret, Component.White, Component.Black);
            }

            // canonicalize order
            if (
                ret.Components.Count == 3 &&
                ret.Components.Contains(Component.Red) &&
                ret.Components.Contains(Component.Green) &&
                ret.Components.Contains(Component.Blue)
            )
            {
                ret = CorrectOrderRGB(ret);
            }
            else if (
                ret.Components.Count == 4 &&
                ret.Components.Contains(Component.Red) &&
                ret.Components.Contains(Component.Green) &&
                ret.Components.Contains(Component.Blue) &&
                ret.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderRGBA(ret);
            }
            else if (
                ret.Components.Count == 2 &&
                ret.Components.Contains(Component.White) &&
                ret.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderWA(ret);
            }
            else if (
                ret.Components.Count == 2 &&
                ret.Components.Contains(Component.Black) &&
                ret.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderBA(ret);
            }
            else if (
                ret.Components.Count == 4 &&
                ret.Components.Contains(Component.Cyan) &&
                ret.Components.Contains(Component.Magenta) &&
                ret.Components.Contains(Component.Yellow) &&
                ret.Components.Contains(Component.Black)
            )
            {
                ret = CorrectOrderCMYK(ret);
            }
            else if (
                ret.Components.Count == 5 &&
                ret.Components.Contains(Component.Cyan) &&
                ret.Components.Contains(Component.Magenta) &&
                ret.Components.Contains(Component.Yellow) &&
                ret.Components.Contains(Component.Black) &&
                ret.Components.Contains(Component.Alpha)
            )
            {
                ret = CorrectOrderCMYKA(ret);
            }

            return ret;
        }
    }
}
