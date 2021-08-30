using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleTcp.Client;
using SimpleTcp.Server.Http;
using System;
using System.Net.NetworkInformation;
using System.Text;

namespace UnitTestProject
{
    [TestClass]
    public class HttpServerTest
    {
        [TestMethod]
        public void StartStopHttpServer_StartWithStartFunctionAndStopWithStopFunction()
        {
            var httpServer = new HttpServer();
            Assert.IsFalse(httpServer.IsStarted);


            httpServer.Start(6000);
            Assert.IsTrue(httpServer.IsStarted, "HttpServer is started by start function");

            httpServer.Stop();

            Assert.IsFalse(httpServer.IsStarted, "HttpServer is stoped by stop function");
        }

        [TestMethod]
        public void ResponseHttpServer_CheckHttpServerResponse()
        {
            var httpServer = new HttpServer();
            Assert.IsFalse(httpServer.IsStarted);

            httpServer.HttpRequest += (sender, e) =>
            {
                return new HttpResponse(HttpStatusCode.OK)
                {
                    Content = Encoding.UTF8.GetBytes("HttpServerTest")
                };
            };

            httpServer.Start(6001);
            Assert.IsTrue(httpServer.IsStarted, "HttpServer is started by start function");

            var client = new RawTcpClient("localhost", 6001);
            Assert.IsTrue(client.IsConnected, "failed to connect httpserver");

            byte[] buffer = Encoding.UTF8.GetBytes("GET / HTTP/1.1\r\n\r\n");
            client.Write(buffer, 0, buffer.Length);

            System.Threading.Thread.Sleep(1000);

            Assert.IsTrue(client.BytesToRead > 0, "no response data");

            byte[] responseBuffer = client.ReadExisting();
            string response = Encoding.UTF8.GetString(responseBuffer);
            Assert.IsTrue(response.Contains("HttpServerTest"), "no response match");

            httpServer.Stop();
            Assert.IsFalse(httpServer.IsStarted, "HttpServer is stoped by stop function");
        }
    }
}
