using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleTcp.Client;
using SimpleTcp.Server;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class PacketTcpServerClientTest
    {
        [TestMethod]
        public void ServerClientTest()
        {
            var packetTcpServer = new PacketTcpServer();
            Assert.IsFalse(packetTcpServer.IsStarted);

            packetTcpServer.Start(5005);
            Assert.IsTrue(packetTcpServer.IsStarted, "PacketTcpServer is started by start function");

            Assert.IsTrue(packetTcpServer.Clients.Length == 0);

            var packetTcpClients = new List<PacketTcpClient>();
            for(int i = 0; i < 25; i++)
            {
                var packetTcpClient = new PacketTcpClient();
                packetTcpClient.Connect("localhost", 5005);
                Assert.IsTrue(packetTcpClient.IsConnected);
                packetTcpClients.Add(packetTcpClient);
            }

            Thread.Sleep(1000);

            Assert.IsTrue(packetTcpServer.Clients.Length == 25, $"Clients.Length({packetTcpServer.Clients.Length}) is not 25");

            for (int i = 0; i < 25; i++)
            {
                var packetTcpClient = new PacketTcpClient("localhost", 5005);
                Assert.IsTrue(packetTcpClient.IsConnected);
                packetTcpClients.Add(packetTcpClient);
            }

            Thread.Sleep(1000);

            Assert.IsTrue(packetTcpServer.Clients.Length == 50, $"Clients.Length({packetTcpServer.Clients.Length}) is not 50");

            Assert.IsTrue(packetTcpServer.TotalReceivedBytes == 0, "packetTcpServer.TotalReceivedBytes is not 0");

            int totalSendBytes = 0;
            int totalSendPackets = 0;
            foreach(var client in packetTcpClients)
            {
                client.WritePacket(new byte[1024]);
                totalSendPackets++;
                totalSendBytes += 1024 + 4;
            }

            Thread.Sleep(1000);

            Assert.IsTrue(packetTcpServer.TotalReceivedPackets == totalSendPackets, $"TotalReceivedPackets({packetTcpServer.TotalReceivedPackets}) is not TotalSendPakcets({totalSendPackets})");

            Assert.IsTrue(packetTcpServer.TotalReceivedBytes == totalSendBytes, $"TotalReceivedBytes({packetTcpServer.TotalReceivedBytes}) is not TotalSendBytes({totalSendBytes}) * Clients.Length");

            packetTcpServer.Stop();
            Assert.IsFalse(packetTcpServer.IsStarted, "PacketTcpServer is stoped by stop function");
        }
    }
}
