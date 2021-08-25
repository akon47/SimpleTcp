using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleTcp.Client;

namespace EchoClient
{
	class Program
	{
		static void Main(string[] args)
		{
            using (RawTcpClient tcpClient = new RawTcpClient())
			{
				tcpClient.Connected += (sender, e) => Console.WriteLine($"Connect to [{e.RemoteEndPoint}]"); 
				tcpClient.Disconnected += (sender, e) => Console.WriteLine($"{Environment.NewLine}Disconnected from [{e.RemoteEndPoint}]");
				tcpClient.DataReceived += (sender, e) =>
				{
                    if (sender is RawTcpClient rawTcpClient)
                    {
                        byte[] readBytes = rawTcpClient.ReadExisting(); // read all data
                        Console.WriteLine($"DataReceived: {Encoding.ASCII.GetString(readBytes)}");
                    }
				};

				try
				{
					tcpClient.Connect("127.0.0.1", 5000);

                    while(true)
                    {
                        string line = Console.ReadLine();
                        byte[] buffer = Encoding.ASCII.GetBytes(line);
                        tcpClient.Write(buffer, 0, buffer.Length);
                    }
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
                    Console.ReadLine();
				}
			}
		}
	}
}
