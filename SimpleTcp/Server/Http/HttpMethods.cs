using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    /// <summary>
    /// Http Methods
    /// https://developer.mozilla.org/ko/docs/Web/HTTP/Methods
    /// </summary>
    public enum HttpMethods
    {
        None,
        Get,
        Head,
        Post,
        Put,
        Delete,
        Connect,
        Options,
        Patch
    }
}
