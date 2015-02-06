using System.IO;
using System.Text;

namespace RavuAlHemio.PbmNet
{
    internal class NetpbmBinaryWriter : BinaryWriter
    {
        private readonly Encoding _encoding;

        public NetpbmBinaryWriter(Stream output, Encoding encoding, bool leaveOpen = false)
            : base(output, encoding, leaveOpen)
        {
            _encoding = encoding;
        }

        public void WriteUnprefixed(string str)
        {
            Write(_encoding.GetBytes(str));
        }

        public void WriteUnprefixed(string format, params object[] objects)
        {
            Write(_encoding.GetBytes(string.Format(format, objects)));
        }
    }
}
