using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTcp.Server;

namespace PacketEchoServer
{
	class Program
	{
		static void Main(string[] args)
		{
			using (PacketTcpServer tcpServer = new PacketTcpServer())
            {
                tcpServer.ClientConnected += (sender, e) =>
                    Console.WriteLine($"[{e}]: Connected"); // new client connected
                tcpServer.ClientDisconnected += (sender, e) =>
                    Console.WriteLine($"[{e}]: Disconnected"); // client disconnected
                tcpServer.PacketReceived += (sender, e) =>
                {
                    if (sender is PacketTcpServer packetTcpServer)
                    {
                        Console.WriteLine($"[{e.Packet.IPEndPoint}]: PacketReceived (PacketLength: {e.Packet.PacketData.Length})");
                        packetTcpServer.WritePacket(e.Packet.TcpClient, e.Packet.PacketData); // return same packet
                    }
                };

                try
                {
                    tcpServer.Start(5000);
                    Console.WriteLine("Listening for connections...");
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
