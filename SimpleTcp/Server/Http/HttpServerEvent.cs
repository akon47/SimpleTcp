using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpRequestEventArgs : EventArgs
    {
        public IHttpRequest Request { get; private set; }

        public HttpRequestEventArgs(IHttpRequest httpRequest)
        {
            Request = httpRequest;
        }
    }
    public delegate IHttpResponse HttpRequestEventHandler(object sender, HttpRequestEventArgs e);
}
