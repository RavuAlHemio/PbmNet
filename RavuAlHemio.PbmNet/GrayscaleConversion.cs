namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// Defines how grayscale colors should be converted.
    /// </summary>
    public enum GrayscaleConversion : byte
    {
        /// <summary>
        /// Grayscale colors remain untouched.
        /// </summary>
        None,

        /// <summary>
        /// Additive grayscale (<see cref="Component.White"/>) components are converted into subtractive grayscale
        /// (<see cref="Component.Black"/>).
        /// </summary>
        /// <remarks>
        /// This is useful for transcoding black-and-white images from <see cref="ImageType.PAM"/> or
        /// <see cref="ImageType.BigPAM"/> to <see cref="ImageType.PBM"/> or <see cref="ImageType.PlainPBM"/>.
        /// </remarks>
        WhiteToBlack,

        /// <summary>
        /// Subtractive grayscale (<see cref="Component.Black"/>) components are converted into additive grayscale
        /// (<see cref="Component.White"/>).
        /// </summary>
        /// <remarks>
        /// This is useful for transcoding black-and-white images from <see cref="ImageType.PBM"/> or
        /// <see cref="ImageType.PlainPBM"/> to <see cref="ImageType.PAM"/> or <see cref="ImageType.BigPAM"/>, or for
        /// visualizing a grayscale printout on a screen.
        /// </remarks>
        BlackToWhite
    }
}
