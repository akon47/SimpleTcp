using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SimpleTcp.Server.Http
{
    public class HttpHeaders : Dictionary<string, string>
    {
        public override string ToString()
        {
            return string.Join("\r\n", this.Select(item => $"{item.Key}: {item.Value}"));
        }
    }
}
