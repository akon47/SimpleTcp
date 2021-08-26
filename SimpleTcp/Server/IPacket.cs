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
        /// <summary>
        /// The client that sent the packet.
        /// </summary>
        TcpClient TcpClient { get; }

        /// <summary>
        /// Address of the client that sent the packet
        /// </summary>
		IPEndPoint IPEndPoint { get; }

        /// <summary>
        /// Packet data
        /// </summary>
        byte[] PacketData { get; }
    }
}
