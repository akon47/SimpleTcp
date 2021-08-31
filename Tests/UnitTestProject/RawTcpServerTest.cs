using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleTcp.Server;
using System;
using System.Net.NetworkInformation;

namespace UnitTestProject
{
    [TestClass]
    public class RawTcpServerTest
    {
        [TestMethod]
        public void StartStopRawTcpServer_StartWithStartFunctionAndStopWithStopFunction()
        {
            var rawTcpServer = new RawTcpServer();
            Assert.IsFalse(rawTcpServer.IsStarted);


            rawTcpServer.Start(0);
            Assert.IsTrue(rawTcpServer.IsStarted, "RawTcpServer is started by start function");

            rawTcpServer.Stop();

            Assert.IsFalse(rawTcpServer.IsStarted, "RawTcpServer is stoped by stop function");
        }

        [TestMethod]
        public void StartStopRawTcpServer_StartWithConstructorAndStopWithStopFunction()
        {
            var rawTcpServer = new RawTcpServer(0);
            Assert.IsTrue(rawTcpServer.IsStarted, "RawTcpServer is started by constructor");

            rawTcpServer.Stop();

            Assert.IsFalse(rawTcpServer.IsStarted, "RawTcpServer is stoped by stop function");

        }
    }
}
