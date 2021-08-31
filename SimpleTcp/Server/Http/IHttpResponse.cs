using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public interface IHttpResponse : IDisposable
    {
        int StatusCode { get; }
        string ReasonPhrase { get; }
        HttpHeaders Headers { get; }
        Stream GetContentStream();
    }
}
