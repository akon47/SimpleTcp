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
		TcpClient TcpClient { get; }
		IPEndPoint IPEndPoint { get; }
		int BytesToRead { get; }
		long DropBytes { get; }
        long SendBytes { get; }
        long ReceivedBytes { get; }
		int Read(byte[] buffer, int offset, int count);
		byte[] ReadExisting();
		int ReadByte();
		void Write(byte[] buffer, int offset, int count);
	}
}
