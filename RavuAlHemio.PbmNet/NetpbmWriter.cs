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
            return SupportedTypesForHeader(img.Header);
        }

        /// <summary>
        /// Returns the set of Netpbm image types into which the image with the given header can be encoded.
        /// </summary>
        /// <returns>The Netpbm image types an image with the given header can be encoded into.</returns>
        /// <param name="header">The header of the image to be encoded.</param>
        /// <typeparam name="TPixelComponent">The pixel component type of the image.</typeparam>
        public ISet<ImageType> SupportedTypesForHeader<TPixelComponent>(NetpbmHeader<TPixelComponent> header)
        {
            // big PAM is always supported
            var types = new HashSet<ImageType> {ImageType.BigPAM};

            if (header.BytesPerPixelComponent > 2)
            {
                // all other formats limit pixel values to two bytes per component
                return types;
            }

            // PAM supports any component types
            types.Add(ImageType.PAM);

            // PPM requires RGB
            if (
                header.Components.Count == 3 &&
                header.Components[0] == Component.Red &&
                header.Components[1] == Component.Green &&
                header.Components[2] == Component.Blue
            )
            {
                types.Add(ImageType.PPM);
                types.Add(ImageType.PlainPPM);
            }

            // PGM requires grayscale (black-to-white)
            if (
                header.Components.Count == 1 &&
                header.Components[0] == Component.White
            )
            {
                types.Add(ImageType.PGM);
                types.Add(ImageType.PlainPGM);
            }

            return types;
        }

        /// <summary>
        /// Returns the PAM tuple name and necessary component permutation for images with the given header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <typeparam name="TPixelComponent">The pixel component type.</typeparam>
        protected virtual string GetPamTupleType<TPixelComponent>(NetpbmHeader<TPixelComponent> header)
        {
            if (
                header.Components.Count == 1 &&
                header.Components[0] == Component.White
            )
            {
                return "GRAYSCALE";
            }
            else if (
                header.Components.Count == 2 &&
                header.Components[0] == Component.White &&
                header.Components[1] == Component.Alpha
            )
            {
                return "GRAYSCALE_ALPHA";
            }
            else if (
                header.Components.Count == 3 &&
                header.Components[0] == Component.Red &&
                header.Components[1] == Component.Green &&
                header.Components[2] == Component.Blue
            )
            {
                return "RGB";
            }
            else if (
                header.Components.Count == 4 &&
                header.Components[0] == Component.Red &&
                header.Components[1] == Component.Green &&
                header.Components[2] == Component.Blue &&
                header.Components[3] == Component.Alpha
            )
            {
                return "RGB_ALPHA";
            }
            else if (
                header.Components.Count == 4 &&
                header.Components[0] == Component.Cyan &&
                header.Components[1] == Component.Magenta &&
                header.Components[2] == Component.Yellow &&
                header.Components[3] == Component.Black
            )
            {
                // "CMYK" is an extension (compatible with GhostScript)
                return "CMYK";
            }
            else if (
                header.Components.Count == 5 &&
                header.Components[0] == Component.Cyan &&
                header.Components[1] == Component.Magenta &&
                header.Components[2] == Component.Yellow &&
                header.Components[3] == Component.Black &&
                header.Components[4] == Component.Alpha
            )
            {
                // "CMYK_ALPHA" is an extension
                return "CMYK_ALPHA";
            }
            else
            {
                // extension: assemble a custom tuple type
                return string.Join("_", header.Components.Select(c => c.ToString().ToUpperInvariant()));
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
            WriteImageHeader(image.Header, stream, type);
            WriteImageData(image, stream, type);
        }

        /// <summary>
        /// Writes the image header into the stream in the given format.
        /// </summary>
        /// <param name="header">The image header to write.</param>
        /// <param name="stream">The stream into which to write the header.</param>
        /// <param name="type">The type of Netpbm image according to which to encode the header.</param>
        /// <typeparam name="TPixelComponent">The type of pixel component values.</typeparam>
        public void WriteImageHeader<TPixelComponent>(NetpbmHeader<TPixelComponent> header, Stream stream, ImageType type)
        {
            // check if the format is supported
            var supportedTypes = SupportedTypesForHeader(header);
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
                    writer.WriteUnprefixed("WIDTH {0}\n", header.Width);

                    // output height line
                    writer.WriteUnprefixed("HEIGHT {0}\n", header.Height);

                    // output number of components
                    writer.WriteUnprefixed("DEPTH {0}\n", header.Components.Count);

                    // maximum value
                    writer.WriteUnprefixed("MAXVAL {0}\n", header.HighestComponentValue);

                    // find the components we are working with
                    writer.WriteUnprefixed("TUPLTYPE {0}\n", GetPamTupleType(header));

                    writer.WriteUnprefixed("ENDHDR\n");
                }
                else
                {
                    // output width and height
                    writer.WriteUnprefixed("{0} {1}\n", header.Width, header.Height);

                    // unless PBM (where the max value is always 1)
                    if (type != ImageType.PBM && type != ImageType.PlainPBM)
                    {
                        // output the max value
                        writer.WriteUnprefixed("{0}\n", header.HighestComponentValue);
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

        /// <summary>
        /// Writes a row of image data into the stream in the given format.
        /// </summary>
        /// <param name="image">The image whose row is being encoded (for formatting purposes).</param>
        /// <param name="row">The row to write.</param>
        /// <param name="stream">The stream to write into.</param>
        /// <param name="type">The format according to which to write.</param>
        public void WriteImageRow<TPixelComponent>(NetpbmImage<TPixelComponent> image, IEnumerable<TPixelComponent> row, Stream stream, ImageType type)
        {
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
                    // output row as decimal numbers
                    // add a newline after the row to make things nicer

                    foreach (var value in row)
                    {
                        writer.WriteUnprefixed(value.ToString());
                        writer.Write(' ');
                    }
                    writer.Write('\n');
                }
                else if (type == ImageType.PBM)
                {
                    // special case: bit-packed format!
                    var values = new List<TPixelComponent>(8);
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
                else
                {
                    // just the big endian bytes
                    foreach (var value in row)
                    {
                        writer.Write(image.ComponentToBigEndianBytes(value).ToArray());
                    }
                }
            }
        }
    }
}
