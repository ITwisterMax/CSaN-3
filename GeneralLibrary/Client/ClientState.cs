using System.Net.Sockets;
using System.Text;

namespace GeneralLibrary.Client
{
    class ClientState
    {
        public const int BufferSize = 65536;
        public Socket WorkSocket { get; set; }
        public byte[] Buffer { get; set; }

        public ClientState()
        {
            WorkSocket = null;
            Buffer = new byte[BufferSize];
        }
    }
}
