using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
	public class PacketReceivedEventArgs : EventArgs
	{
		public IPacket Packet { get; private set; }

		public PacketReceivedEventArgs(IPacket packet)
		{
			Packet = packet;
		}
	}
	public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);
}
