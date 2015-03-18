using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;

namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Writes Netpbm images to streams.
    /// </summary>
    public class NetpbmWriter
    {
        /// <summary>
        /// Returns the set of Netpbm image types into which the given image can be encoded.
        /// </summary>
        /// <returns>The Netpbm image types the given image can be encoded into.</returns>
        /// <param name="img">The image to be encoded.</param>
        /// <typeparam name="TPixelComponent">The pixel component type of the image.</typeparam>
        public ISet<ImageType> SupportedTypesForImage<TPixelComponent>(NetpbmImage<TPixelComponent> img)
        {
            // big PAM is always supported
            var types = new HashSet<ImageType> {ImageType.BigPAM};

            if (img.Header.BytesPerPixelComponent > 2)
            {
                // all other formats limit pixel values to two bytes per component
                return types;
            }

            // PAM supports any component types
            types.Add(ImageType.PAM);

            // PPM requires RGB
            if (
                img.Header.Components.Count == 3 &&
                img.Header.Components[0] == Component.Red &&
                img.Header.Components[1] == Component.Green &&
                img.Header.Components[2] == Component.Blue
            )
            {
                types.Add(ImageType.PPM);
                types.Add(ImageType.PlainPPM);
            }

            // PGM requires grayscale (black-to-white)
            if (
                img.Header.Components.Count == 1 &&
                img.Header.Components[0] == Component.White
            )
            {
                types.Add(ImageType.PGM);
                types.Add(ImageType.PlainPGM);
            }

            return types;
        }

        /// <summary>
        /// Returns the PAM tuple name and necessary component permutation for the given image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <typeparam name="TPixelComponent">The pixel component type.</typeparam>
        protected virtual string GetPamTupleType<TPixelComponent>(NetpbmImage<TPixelComponent> image)
        {
            if (
                image.Header.Components.Count == 1 &&
                image.Header.Components[0] == Component.White
            )
            {
                return "GRAYSCALE";
            }
            else if (
                image.Header.Components.Count == 2 &&
                image.Header.Components[0] == Component.White &&
                image.Header.Components[1] == Component.Alpha
            )
            {
                return "GRAYSCALE_ALPHA";
            }
            else if (
                image.Header.Components.Count == 3 &&
                image.Header.Components[0] == Component.Red &&
                image.Header.Components[1] == Component.Green &&
                image.Header.Components[2] == Component.Blue
            )
            {
                return "RGB";
            }
            else if (
                image.Header.Components.Count == 4 &&
                image.Header.Components[0] == Component.Red &&
                image.Header.Components[1] == Component.Green &&
                image.Header.Components[2] == Component.Blue &&
                image.Header.Components[3] == Component.Alpha
            )
            {
                return "RGB_ALPHA";
            }
            else if (
                image.Header.Components.Count == 4 &&
                image.Header.Components[0] == Component.Cyan &&
                image.Header.Components[1] == Component.Magenta &&
                image.Header.Components[2] == Component.Yellow &&
                image.Header.Components[3] == Component.Black
            )
            {
                // "CMYK" is an extension (compatible with GhostScript)
                return "CMYK";
            }
            else if (
                image.Header.Components.Count == 5 &&
                image.Header.Components[0] == Component.Cyan &&
                image.Header.Components[1] == Component.Magenta &&
                image.Header.Components[2] == Component.Yellow &&
                image.Header.Components[3] == Component.Black &&
                image.Header.Components[4] == Component.Alpha
            )
            {
                // "CMYK_ALPHA" is an extension
                return "CMYK_ALPHA";
            }
            else
            {
                // extension: assemble a custom tuple type
                return string.Join("_", image.Header.Components.Select(c => c.ToString().ToUpperInvariant()));
            }
        }

        /// <summary>
        /// Encodes up to eight bitmap values into a byte. The values are stored from the most significant bit downward,
        /// i.e. the leftmost value is stored in the most significant bit.
        /// </summary>
        /// <returns>The bitmap pixels encoded into a byte.</returns>
        /// <param name="image">The image being encoded.</param>
        /// <param name="pixels">The bitmap pixels to encode into a byte.</param>
        /// <typeparam name="TPixelComponent">The type of the pixel component.</typeparam>
        private byte EncodeBitmapValuesIntoByte<TPixelComponent>(NetpbmImage<TPixelComponent> image, IList<TPixelComponent> pixels)
        {
            Contract.Assert(pixels.Count > 0 && pixels.Count < 9);

            byte theByte = 0;

            // earliest value = most significant bit
            for (int i = 0; i < 8; ++i)
            {
                if (!image.IsComponentValueZero(pixels[i]))
                {
                    theByte |= (byte)(1 << (7 - i));
                }
            }

            return theByte;
        }

        /// <summary>
        /// Writes the image into the stream in the given format.
        /// </summary>
        /// <param name="image">The image to write.</param>
        /// <param name="stream">The stream into which to write the image.</param>
        /// <param name="type">The type of Netpbm image into which to encode the image.</param>
        /// <typeparam name="TPixelComponent">The type of pixel component values.</typeparam>
        public void WriteImage<TPixelComponent>(NetpbmImage<TPixelComponent> image, Stream stream, ImageType type)
        {
            WriteImageHeader(image, stream, type);
            WriteImageData(image, stream, type);
        }

        /// <summary>
        /// Writes the image header into the stream in the given format.
        /// </summary>
        /// <param name="image">The image whose header to write.</param>
        /// <param name="stream">The stream into which to write the header.</param>
        /// <param name="type">The type of Netpbm image according to which to encode the header.</param>
        /// <typeparam name="TPixelComponent">The type of pixel component values.</typeparam>
        public void WriteImageHeader<TPixelComponent>(NetpbmImage<TPixelComponent> image, Stream stream, ImageType type)
        {
            // check if the format is supported
            var supportedTypes = SupportedTypesForImage(image);
            if (!supportedTypes.Contains(type))
            {
                throw new ArgumentOutOfRangeException("type", type, "the image cannot be encoded into this type");
            }

            using (var writer = new NetpbmBinaryWriter(stream, new UTF8Encoding(false, true), leaveOpen: true))
            {
                int magicNumber = (int)type;
                switch (type)
                {
                    case ImageType.PBM:
                    case ImageType.PGM:
                    case ImageType.PPM:
                    case ImageType.PAM:
                    case ImageType.PlainPBM:
                    case ImageType.PlainPGM:
                    case ImageType.PlainPPM:
                        break;
                    case ImageType.BigPAM:
                        magicNumber = (int)ImageType.PAM;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type", type, "unknown image type");
                }

                // output the magic
                writer.WriteUnprefixed("P{0}\n", magicNumber);

                if (type == ImageType.PAM || type == ImageType.BigPAM)
                {
                    // output width line
                    writer.WriteUnprefixed("WIDTH {0}\n", image.Header.Width);

                    // output height line
                    writer.WriteUnprefixed("HEIGHT {0}\n", image.Header.Height);

                    // output number of components
                    writer.WriteUnprefixed("DEPTH {0}\n", image.Header.Components.Count);

                    // maximum value
                    writer.WriteUnprefixed("MAXVAL {0}\n", image.Header.HighestComponentValue);

                    // find the components we are working with
                    writer.WriteUnprefixed("TUPLTYPE {0}\n", GetPamTupleType(image));

                    writer.WriteUnprefixed("ENDHDR\n");
                }
                else
                {
                    // output width and height
                    writer.WriteUnprefixed("{0} {1}\n", image.Header.Width, image.Header.Height);

                    // unless PBM (where the max value is always 1)
                    if (type != ImageType.PBM && type != ImageType.PlainPBM)
                    {
                        // output the max value
                        writer.WriteUnprefixed("{0}\n", image.Header.HighestComponentValue);
                    }

                    // the header's final newline
                    writer.Write('\n');
                }
            }
        }

        /// <summary>
        /// Writes the image data into the stream in the given format.
        /// </summary>
        /// <param name="image">The image whose data to write.</param>
        /// <param name="stream">The stream into which to write the data.</param>
        /// <param name="type">The type of Netpbm image according to which to encode the data.</param>
        /// <typeparam name="TPixelComponent">The type of pixel component values.</typeparam>
        public void WriteImageData<TPixelComponent>(NetpbmImage<TPixelComponent> image, Stream stream, ImageType type)
        {
            // check if the format is supported
            var supportedTypes = SupportedTypesForImage(image);
            if (!supportedTypes.Contains(type))
            {
                throw new ArgumentOutOfRangeException("type", type, "the image cannot be encoded into this type");
            }

            using (var writer = new NetpbmBinaryWriter(stream, new UTF8Encoding(false, true), leaveOpen: true))
            {
                bool isPlain;
                switch (type)
                {
                    case ImageType.PBM:
                    case ImageType.PGM:
                    case ImageType.PPM:
                    case ImageType.PAM:
                    case ImageType.BigPAM:
                        isPlain = false;
                        break;
                    case ImageType.PlainPBM:
                    case ImageType.PlainPGM:
                    case ImageType.PlainPPM:
                        isPlain = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type", type, "unknown image type");
                }

                if (isPlain)
                {
                    // output in row-major order as decimal numbers
                    // add newlines after each row to make things nicer

                    foreach (var row in image.NativeRows)
                    {
                        foreach (var value in row)
                        {
                            writer.WriteUnprefixed(value.ToString());
                            writer.Write(' ');
                        }
                        writer.Write('\n');
                    }
                }
                else if (type == ImageType.PBM)
                {
                    // special case: bit-packed format!
                    var values = new List<TPixelComponent>(8);
                    foreach (var row in image.NativeRows)
                    {
                        foreach (var value in row)
                        {
                            values.Add(value);

                            if (values.Count == 8)
                            {
                                byte theByte = EncodeBitmapValuesIntoByte(image, values);
                                writer.Write(theByte);
                                values.Clear();
                            }
                        }

                        if (values.Count != 0)
                        {
                            byte theByte = EncodeBitmapValuesIntoByte(image, values);
                            writer.Write(theByte);
                        }
                    }
                }
                else
                {
                    // just the big endian bytes
                    foreach (var row in image.NativeRows)
                    {
                        foreach (var value in row)
                        {
                            writer.Write(image.ComponentToBigEndianBytes(value).ToArray());
                        }
                    }
                }
            }
        }
    }
}
