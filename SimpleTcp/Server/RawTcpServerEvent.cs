using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
	public class ClientConnectedEventArgs : EventArgs
	{
		public IClient Client { get; private set; }

		public ClientConnectedEventArgs(IClient client)
		{
			Client = client;
		}
	}

	public delegate void ClientConnectedHandler(object sender, ClientConnectedEventArgs e);


	public class ClientDisconnectedEventArgs : EventArgs
	{
		public IClient Client { get; private set; }

		public ClientDisconnectedEventArgs(IClient client)
		{
			Client = client;
		}
	}

	public delegate void ClientDisconnectedHandler(object sender, ClientDisconnectedEventArgs e);

	public class DataReceivedEventArgs : EventArgs
	{
		public IClient Client { get; private set; }

		public DataReceivedEventArgs(IClient client)
		{
			Client = client;
		}
	}
	public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
