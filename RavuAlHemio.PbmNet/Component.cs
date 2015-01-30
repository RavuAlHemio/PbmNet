namespace RavuAlHemio.PbmNet
{
    /// <summary>
    /// What type of component this is.
    /// </summary>
    public enum Component
    {
        /// <summary>
        /// An unknown type of component.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The gray component in a grayscale image, or the black
        /// component in a CMYK image. The lowest value is black,
        /// the highest value is white.
        /// </summary>
        Gray = 1,
        
        /// <summary>
        /// The red component in an RGB image.
        /// </summary>
        Red = 2,

        /// <summary>
        /// The green component in an RGB image.
        /// </summary>
        Green = 3,

        /// <summary>
        /// The blue component in an RGB image.
        /// </summary>
        Blue = 4,

        /// <summary>
        /// The cyan component in a CMY or CMYK image.
        /// </summary>
        Cyan = 5,

        /// <summary>
        /// The magenta component in a CMY or CMYK image.
        /// </summary>
        Magenta = 6,

        /// <summary>
        /// The yellow component in a CMY or CMYK image.
        /// </summary>
        Yellow = 7,

        /// <summary>
        /// The alpha component in an image with transparency.
        /// </summary>
        Alpha = 8
    }
}
