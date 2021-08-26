using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
	public class PacketReceivedEventArgs : EventArgs
	{
		public byte[] Data { get; private set; }

		public PacketReceivedEventArgs(byte[] data)
		{
			Data = data;
		}
	}
	public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);
}
