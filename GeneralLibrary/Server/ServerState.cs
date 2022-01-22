using System.Net.Sockets;

namespace GeneralLibrary.Server
{
    class ServerState
    {
        public const int BufferSize = 65536;
        public Socket WorkSocket { get; set; }
        public byte[] Buffer { get; set; }
        public string pcMachineName { get; set; }
        public string pcUserName { get; set; }
        public string pcUserDomainName { get; set; }
        public string pcOSVersion { get; set; }

        public ServerState()
        {
            WorkSocket = null;
            Buffer = new byte[BufferSize];
            pcMachineName = string.Empty;
            pcUserName = string.Empty;
            pcUserDomainName = string.Empty;
            pcOSVersion = string.Empty;
        }
    }
}
