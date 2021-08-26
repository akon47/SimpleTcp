using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
    public class ClientConnectedEventArgs : EventArgs
	{
		public TcpClient TcpClient { get; private set; }
		public IPEndPoint IPEndPoint { get; private set; }

		public ClientConnectedEventArgs(IClient client)
		{
			TcpClient = client.TcpClient;
            IPEndPoint = client.IPEndPoint;
		}

        public override string ToString()
        {
            return IPEndPoint?.ToString();
        }
    }

	public delegate void ClientConnectedHandler(object sender, ClientConnectedEventArgs e);


	public class ClientDisconnectedEventArgs : EventArgs
	{
		public TcpClient TcpClient { get; private set; }
		public IPEndPoint IPEndPoint { get; private set; }

		public ClientDisconnectedEventArgs(IClient client)
		{
			TcpClient = client.TcpClient;
            IPEndPoint = client.IPEndPoint;
		}

        public override string ToString()
        {
            return IPEndPoint?.ToString();
        }
	}

	public delegate void ClientDisconnectedHandler(object sender, ClientDisconnectedEventArgs e);
}
