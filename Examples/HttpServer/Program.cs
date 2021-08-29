using SimpleTcp.Server.Http;
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
                httpServer.HttpRequest += (sender, e) =>
                {
                    Console.WriteLine($"[{e.Request.IPEndPoint}] -> [{e.Request.Url}]");
                    switch(e.Request.Url)
                    {
                        case "/":
                            return new HttpResponse(HttpStatusCode.OK)
                            {
                                Content = Encoding.UTF8.GetBytes(
                                    "<!DOCTYPE html>" +
                                    "<html>" +
                                    "<head>" +
                                    "<meta charset=\"UTF-8\">" +
                                    "<title>SimpleTcp HttpServer Example</title>" +
                                    "</head>" +
                                    "<body>" +
                                    "Hello, World !!<br/><a href=\"https://github.com/akon47/SimpleTcp\">Github</a>" +
                                    "</body>" +
                                    "</html>")
                            };
                        default:
                            return new HttpResponse(HttpStatusCode.NotFound);
                    }
                };
                httpServer.Start();

                Console.ReadLine();
            }
        }
    }
}
