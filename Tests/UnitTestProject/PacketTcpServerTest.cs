using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleTcp.Server;
using System;
using System.Net.NetworkInformation;

namespace UnitTestProject
{
    [TestClass]
    public class PacketTcpServerTest
    {
        [TestMethod]
        public void StartStopPacketTcpServer_StartWithStartFunctionAndStopWithStopFunction()
        {
            var packetTcpServer = new PacketTcpServer();
            Assert.IsFalse(packetTcpServer.IsStarted);


            packetTcpServer.Start(0);
            Assert.IsTrue(packetTcpServer.IsStarted, "PacketTcpServer is started by start function");

            packetTcpServer.Stop();

            Assert.IsFalse(packetTcpServer.IsStarted, "PacketTcpServer is stoped by stop function");
        }

        [TestMethod]
        public void StartStopPacketTcpServer_StartWithConstructorAndStopWithStopFunction()
        {
            var packetTcpServer = new RawTcpServer(0);
            Assert.IsTrue(packetTcpServer.IsStarted, "PacketTcpServer is started by constructor");

            packetTcpServer.Stop();

            Assert.IsFalse(packetTcpServer.IsStarted, "PacketTcpServer is stoped by stop function");

        }
    }
}
