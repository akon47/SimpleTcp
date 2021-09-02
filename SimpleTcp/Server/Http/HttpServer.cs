using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpServer : BaseTcpServer
    {
        #region PrivateMember
        private object syncObject = new object();
        private Dictionary<TcpClient, HttpRequest> requests = new Dictionary<TcpClient, HttpRequest>();
        #endregion

        #region Public Member
        /// <summary>
        /// Called when request from client.
        /// </summary>
		public event HttpRequestEventHandler HttpRequest;
        #endregion

        #region Public Methods

        #region Constructor
        /// <summary>
        /// RawTcpServer
        /// </summary>
        /// <param name="port">If you specify a valid port, the server starts immediately.</param>
        public HttpServer(int port = -1) : base(port) { }
        #endregion

        public void Start()
        {
            base.Start(80);
        }
        #endregion

        #region Protected Methods
        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            HttpRequest httpRequest = null;
            lock (syncObject)
            {
                if (!requests.ContainsKey(client.TcpClient))
                {
                    httpRequest = new HttpRequest(client.TcpClient);
                    requests.Add(client.TcpClient, httpRequest);
                }
                else
                {
                    httpRequest = requests[client.TcpClient];
                }
            }

            while (client.BytesToRead > 0)
            {
                if(httpRequest.Process(client))
                {
                    IHttpResponse httpResponse = HttpRequest?.Invoke(this, new HttpRequestEventArgs(httpRequest));
                    if(httpResponse != null)
                    {
                        WriteHttpResponse(httpResponse, client);
                        httpResponse.Dispose();
                    }
                    client.Disconnect();
                    break;
                }
            }
        }

        protected override void OnClientDisconnected(IClient client)
        {
            lock (syncObject)
            {
                try
                {
                    if (requests.ContainsKey(client.TcpClient))
                    {
                        requests.Remove(client.TcpClient);
                    }
                }
                catch { }
            }
        }
        #endregion

        #region Private Methods
        private void WriteHttpResponse(IHttpResponse httpResponse, IClient client)
        {
            if(!httpResponse.Headers.ContainsKey("content-type"))
            {
                httpResponse.Headers.Add("content-type", "text/html");
            }

            Stream contentStream = httpResponse.GetContentStream();
            httpResponse.Headers["content-length"] = $"{contentStream?.Length ?? 0}";

            WriteText(client, $"HTTP/1.1 {httpResponse.StatusCode} {httpResponse.ReasonPhrase}\r\n");
            WriteText(client, httpResponse.Headers.ToString());
            WriteText(client, "\r\n\r\n"); // end

            if(contentStream?.Length > 0)
            {
                byte[] buffer = new byte[1024 * 4];

                int readBytes;
                while((readBytes = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    client.Write(buffer, 0, readBytes);
                }
            }
        }

        private void WriteText(IClient client, string text, Encoding encoding = null)
        {
            byte[] buffer = (encoding ?? Encoding.UTF8).GetBytes(text);
            client.Write(buffer, 0, buffer.Length);
        }
        #endregion
    }
}
