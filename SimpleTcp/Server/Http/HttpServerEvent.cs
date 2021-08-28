using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpRequestEventArgs : EventArgs
    {

    }
    public delegate void HttpRequestEventHandler(object sender, HttpRequestEventArgs e);
}
