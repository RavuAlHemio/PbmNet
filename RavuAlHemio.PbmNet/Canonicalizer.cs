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
        public Canonicalizer()
        {
        }
    }
}

