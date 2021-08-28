using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
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
