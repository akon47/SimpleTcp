using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTcp.Server;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (RawTcpServer tcpServer = new RawTcpServer())
            {
                tcpServer.ClientConnected += (sender, e) =>
                    Console.WriteLine($"[{e.Client}]: Connected"); // new client connected
                tcpServer.ClientDisconnected += (sender, e) =>
                    Console.WriteLine($"[{e.Client}]: Disconnected"); // client disconnected
                tcpServer.DataReceived += (sender, e) =>
                {
                    byte[] readBytes = e.Client.ReadExisting(); // read all data
                    string dataString = readBytes.Aggregate( // data to hex string
                        new StringBuilder(32),
                        (stringBuilder, data) => stringBuilder.Append($" 0x{data.ToString("X2")}")
                        ).ToString().Trim();

                    Console.WriteLine($"[{e.Client}]: {dataString}");

                    e.Client.Write(readBytes, 0, readBytes.Length); // return same data
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
