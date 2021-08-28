using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server
{
    public class HttpRequestEventArgs : EventArgs
    {

    }
    public delegate void HttpRequestEventHandler(object sender, HttpRequestEventArgs e);
}
