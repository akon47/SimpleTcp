using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
    public interface IPacket
    {
        TcpClient TcpClient { get; }
		IPEndPoint IPEndPoint { get; }
        byte[] PacketData { get; }
    }
}
