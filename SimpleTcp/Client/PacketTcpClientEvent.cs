using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
    public class PacketReceivedEventArgs : EventArgs
	{
		public byte[] PacketData { get; private set; }

		public PacketReceivedEventArgs(byte[] packetData)
		{
			PacketData = packetData;
		}
	}
	public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);
}
