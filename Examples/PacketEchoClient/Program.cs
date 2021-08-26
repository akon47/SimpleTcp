using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTcp.Client;

namespace PacketEchoClient
{
	class Program
	{
		static void Main(string[] args)
        {
            using (PacketTcpClient tcpClient = new PacketTcpClient())
            {
                tcpClient.Connected += (sender, e) =>
                    Console.WriteLine($"Connect to [{e.RemoteEndPoint}]");
                tcpClient.Disconnected += (sender, e) =>
                    Console.WriteLine($"{Environment.NewLine}Disconnected from [{e.RemoteEndPoint}]");
                tcpClient.PacketReceived += (sender, e) =>
                {
                    Console.WriteLine($"PacketReceived: (PacketLength: {e.PacketData.Length})");
                };

                try
                {
                    tcpClient.Connect("127.0.0.1", 5000);

                    tcpClient.WritePacket(new byte[1024]); // send 1024 bytes
                    tcpClient.WritePacket(new byte[1024 * 1024]); // send 1024 * 1024 bytes
                    tcpClient.WritePacket(new byte[1024 * 1024 * 10]); // send 1024 * 1024 * 10 bytes

                    Console.ReadLine();

                    tcpClient.Disconnect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Console.ReadLine();
            }
        }
	}
}
