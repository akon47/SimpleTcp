using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
	public class DataReceivedEventArgs : EventArgs
	{
		public int ReceivedBytes { get; private set; }

        public DataReceivedEventArgs(int receivedBytes)
        {
            ReceivedBytes = receivedBytes;
        }
	}

	public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
}
