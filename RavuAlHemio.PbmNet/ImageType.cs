namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// A type of a Netpbm image.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// The PBM (Portable Bi-level Map) image format. Represents black-and-white (1-bit) images using a packed
        /// binary encoding.
        /// </summary>
        PBM = 4,

        /// <summary>
        /// The Plain PBM (Portable Bi-level Map) image format. Represents black-and-white (1-bit) images using a
        /// plain-text encoding.
        /// </summary>
        PlainPBM = 1,

        /// <summary>
        /// The PGM (Portable Gray Map) image format. Represents grayscale images using a binary encoding.
        /// </summary>
        PGM = 5,

        /// <summary>
        /// The Plain PGM (Portable Gray Map) image format. Represents grayscale images using a plain-text encoding.
        /// </summary>
        PlainPGM = 2,

        /// <summary>
        /// The PPM (Portable Pixel Map) image format. Represents RGB (red-green-blue) images using a binary encoding.
        /// </summary>
        PPM = 6,

        /// <summary>
        /// The Plain PPM (Portable Pixel Map) image format. Represents RGB (red-green-blue) images using a plain-text
        /// encoding.
        /// </summary>
        PlainPPM = 3,

        /// <summary>
        /// The PAM (Portable Arbitrary Map) image format. Represents effectively any kind of image using a binary
        /// encoding. The maximum pixel component value is constrained to <value>65535</value>.
        /// </summary>
        PAM = 7,

        /// <summary>
        /// The unofficial Big PAM (Portable Arbitrary Map) image format. Represents effectively any kind of image using
        /// a binary encoding. The maximum pixel component value is unconstrained.
        /// </summary>
        BigPAM = 70
    }
}
