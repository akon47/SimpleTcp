using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
	public interface IClient
	{
        /// <summary>
        /// Client
        /// </summary>
		TcpClient TcpClient { get; }

        /// <summary>
        /// The client's address.
        /// </summary>
		IPEndPoint IPEndPoint { get; }

        /// <summary>
        /// Gets the number of bytes of data in the receive buffer.
        /// </summary>
		int BytesToRead { get; }

        /// <summary>
        /// The data in the receive buffer overflowed to get the number of bytes lost.
        /// </summary>
		long DropBytes { get; }

        /// <summary>
        /// Get the total number of bytes sent to that client.
        /// </summary>
        long SendedBytes { get; }

        /// <summary>
        /// Get the total number of bytes received from the client.
        /// </summary>
        long ReceivedBytes { get; }


		int Read(byte[] buffer, int offset, int count);
		byte[] ReadExisting();
		int ReadByte();
		void Write(byte[] buffer, int offset, int count);
        void Disconnect();
	}
}
