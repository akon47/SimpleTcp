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
    public class RawTcpServerClientTest
    {
        [TestMethod]
        public void ServerClientTest()
        {
            var rawTcpServer = new RawTcpServer();
            Assert.IsFalse(rawTcpServer.IsStarted);

            rawTcpServer.Start(0);
            Assert.IsTrue(rawTcpServer.IsStarted, "RawTcpServer is started by start function");

            Assert.IsTrue(rawTcpServer.Clients.Length == 0);

            var rawTcpClients = new List<RawTcpClient>();
            for(int i = 0; i < 25; i++)
            {
                var rawTcpClient = new RawTcpClient();
                rawTcpClient.Connect("localhost", rawTcpServer.Port);
                Assert.IsTrue(rawTcpClient.IsConnected);
                rawTcpClients.Add(rawTcpClient);
            }

            Thread.Sleep(1000);

            Assert.IsTrue(rawTcpServer.Clients.Length == 25, $"Clients.Length({rawTcpServer.Clients.Length}) is not 25");

            for (int i = 0; i < 25; i++)
            {
                var rawTcpClient = new RawTcpClient("localhost", rawTcpServer.Port);
                Assert.IsTrue(rawTcpClient.IsConnected);
                rawTcpClients.Add(rawTcpClient);
            }

            Thread.Sleep(1000);

            Assert.IsTrue(rawTcpServer.Clients.Length == 50, $"Clients.Length({rawTcpServer.Clients.Length}) is not 50");

            rawTcpServer.Stop();
            Assert.IsFalse(rawTcpServer.IsStarted, "RawTcpServer is stoped by stop function");
        }
    }
}
