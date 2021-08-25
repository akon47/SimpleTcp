using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
	public class DataReceivedEventArgs : EventArgs
	{
		public IServer Server { get; private set; }

		public DataReceivedEventArgs(IServer server)
		{
			Server = server;
		}
	}

	public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
