using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var httpServer = new SimpleTcp.Server.Http.HttpServer())
            {
                httpServer.Start();

                Console.ReadLine();
            }
        }
    }
}
