using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public interface IHttpRequest
    {
        IPEndPoint IPEndPoint { get; }
        HttpMethods Method { get; }
        HttpHeaders Headers { get; }
        string Url { get; }
        byte[] Content { get; }

    }
}
