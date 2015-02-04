using System.IO;

namespace RavuAlHemio.PbmNet
{
    internal static class NetpbmUtil
    {
        /// <summary>
        /// Reads from a stream until end-of-file or the buffer is filled, and returns whether the buffer was filled.
        /// </summary>
        /// <returns><c>true</c> if the buffer was filled; <c>false</c> if end-of-file was encountered.</returns>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="buffer">The buffer to fill.</param>
        public static bool ReadToFillBuffer(Stream stream, byte[] buffer)
        {
            int offset = 0;
            for (; ; )
            {
                int remainingBytes = buffer.Length - offset;
                if (remainingBytes == 0)
                {
                    return true;
                }
                var readBytes = stream.Read(buffer, offset, remainingBytes);
                if (readBytes == 0)
                {
                    // EOF
                    return false;
                }
                offset += readBytes;
            }
        }
    }
}
