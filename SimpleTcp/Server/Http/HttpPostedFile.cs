using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpPostedFile
    {
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public Stream FileStream { get; private set; }
    }
}
