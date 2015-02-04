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
        /// The white component in an additive grayscale image. The lowest value is black, the highest value is white.
        /// </summary>
        White = 1,
        
        /// <summary>
        /// The red component in an RGB image. The lowest value is "no red", the highest is "full red".
        /// </summary>
        Red = 2,

        /// <summary>
        /// The green component in an RGB. The lowest value is "no green", the highest is "full green".
        /// </summary>
        /// <remarks>
        /// This is the additive variant of green. The subtractive variant is <see cref="SubtractiveGreen"/>.
        /// </remarks>
        Green = 3,

        /// <summary>
        /// The blue component in an RGB image. The lowest value is "no blue", the highest is "full blue".
        /// </summary>
        Blue = 4,

        /// <summary>
        /// The cyan component in a CMY or CMYK image. The lowest value is "no cyan", the highest is "full cyan".
        /// </summary>
        Cyan = 5,

        /// <summary>
        /// The magenta component in a CMY or CMYK image. The lowest value is "no magenta", the highest is "full
        /// magenta".
        /// </summary>
        Magenta = 6,

        /// <summary>
        /// The yellow component in a CMY or CMYK image. The lowest value is "no yellow", the highest is "full yellow".
        /// </summary>
        Yellow = 7,

        /// <summary>
        /// The black component in a subtractive grayscale or CMYK image. The lowest value is "no black", the highest is
        /// "full black".
        /// </summary>
        Black = 8,

        /// <summary>
        /// The alpha component in an image with transparency. The lowest value is "completely transparent", the highest
        /// is "fully opaque".
        /// </summary>
        Alpha = 9,

        /// <summary>
        /// The orange component in a CMYKOG image. The lowest value is "no orange", the highest is "full orange".
        /// </summary>
        Orange = 10,

        /// <summary>
        /// The green component in a CMYKOG image. The lowest value is "no green", the highest is "full green".
        /// </summary>
        /// <remarks>
        /// The more common additive variant of green is <see cref="Green"/>.
        /// </remarks>
        SubtractiveGreen = 11,

        /// <summary>
        /// The light cyan component in a CcMmYK image. The lowest value is "no light cyan", the highest is "full light
        /// cyan".
        /// </summary>
        LightCyan = 12,

        /// <summary>
        /// The light magenta component in a CcMmYK image. The lowest value is "no light magenta", the highest is "full
        /// light magenta".
        /// </summary>
        LightMagenta = 13
    }
}
