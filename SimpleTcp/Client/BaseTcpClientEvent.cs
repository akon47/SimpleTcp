using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
    public class ConnectedEventArgs : EventArgs
	{
		public IPEndPoint RemoteEndPoint { get; private set; }

        public ConnectedEventArgs(TcpClient tcpClient)
        {
            RemoteEndPoint = tcpClient?.Client?.RemoteEndPoint as IPEndPoint;
        }
	}

	public delegate void ConnectedHandler(object sender, ConnectedEventArgs e);


	public class DisconnectedEventArgs : EventArgs
	{
        public IPEndPoint RemoteEndPoint { get; private set; }

        public DisconnectedEventArgs(IPEndPoint remoteEndPoint)
        {
            RemoteEndPoint = remoteEndPoint;
        }
	}

	public delegate void DisconnectedHandler(object sender, DisconnectedEventArgs e);
}
