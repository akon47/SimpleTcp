﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public interface IHttpRequest
    {
        HttpMethods Method { get; }
        HttpHeaders Headers { get; }
    }
}
