using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpRequest : IHttpRequest
    {
        #region Properties
        public TcpClient TcpClient { get; private set; }
        public IPEndPoint IPEndPoint { get => TcpClient?.Client?.RemoteEndPoint as IPEndPoint; }
        public HttpMethods Method { get; private set; } = HttpMethods.None;
        public string Url { get; private set; } = String.Empty;
        public HttpHeaders Headers { get; private set; } = new HttpHeaders();
        public byte[] Content { get; private set; }
        #endregion

        #region Private Members
        private object syncObject = new object();
        private List<string> request = new List<string>();
        private int contentLength = 0;
        private int contentWritePosition = -1;
        #endregion

        public HttpRequest(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            request.Add(String.Empty);
        }

        public bool Process(IClient client)
        {
            lock (syncObject)
            {
                if (Content != null) // fill content bytes
                {
                    int remain = Content.Length - contentWritePosition;
                    if(remain > 0)
                    {
                        contentWritePosition += client.Read(Content, contentWritePosition, remain);
                        return (Content.Length == contentWritePosition);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    int character = client.ReadByte();
                    while (character > 0)
                    {
                        switch (character)
                        {
                            case '\r':
                                character = client.ReadByte();
                                continue;
                            case '\n':
                                if (string.IsNullOrWhiteSpace(request[request.Count - 1]))
                                {
                                    ParseRequest();
                                    return (Content != null && Content.Length == contentWritePosition);
                                }
                                else
                                {
                                    request.Add(String.Empty);
                                    character = client.ReadByte();
                                    continue;
                                }
                        }

                        request[request.Count - 1] += Convert.ToChar(character);
                        character = client.ReadByte();
                    }
                }
            }
            return false;
        }

        private void ParseRequest()
        {
            for(int i = 0; i < request.Count; i++)
            {
                switch (i)
                {
                    case 0: // parse method, url, http version
                        #region parse method, url, http version
                        string[] startRequest = request[i].Split(' ');
                        switch (startRequest[0].ToLower())
                        {
                            case "get":
                                Method = HttpMethods.Get;
                                break;
                            case "head":
                                Method = HttpMethods.Head;
                                break;
                            case "post":
                                Method = HttpMethods.Post;
                                break;
                            case "put":
                                Method = HttpMethods.Put;
                                break;
                            case "delete":
                                Method = HttpMethods.Delete;
                                break;
                            case "connect":
                                Method = HttpMethods.Connect;
                                break;
                            case "options":
                                Method = HttpMethods.Options;
                                break;
                            case "patch":
                                Method = HttpMethods.Patch;
                                break;
                            default:
                                Method = HttpMethods.None;
                                break;
                        }
                        Url = startRequest[1];
                        #endregion
                        break;
                    default: // parse headers
                        #region parse headers
                        int index = request[i].IndexOf(':');
                        if(index >= 0)
                        {
                            string headerName = request[i].Substring(0, index).Trim();
                            string headerValue = request[i].Substring(index + 1).Trim();
                            Headers.Add(headerName, headerValue);
                        }
                        #endregion
                        break;
                }
            }

            if(Headers.ContainsKey("Content-Length"))
            {
                contentLength = Convert.ToInt32(Headers["Content-Length"]);
                Content = new byte[contentLength];
                contentWritePosition = 0;
            }
            else
            {
                Content = new byte[0];
                contentLength = 0;
                contentWritePosition = 0;
            }
        }
    }
}
