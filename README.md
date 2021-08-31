![logo](https://raw.githubusercontent.com/akon47/SimpleTcp/master/SimpleTcp/logo.png)

# SimpleTcp
<p>
    <img alt="Hits" src="https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2Fakon47%2FSimpleTcp&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false" />
  <img alt="MIT license" src="https://img.shields.io/badge/License-MIT-green.svg">
  <img alt="Nuget version" src="https://img.shields.io/nuget/v/SimpleTcpLib">
  <img alt="Nuget downloads" src="https://img.shields.io/nuget/dt/SimpleTcpLib">
  <img alt="GitHub starts" src="https://img.shields.io/github/stars/akon47/SimpleTcp">
</p>
A simple-to-use TCP server and client library.   

## 🎨 Features
- Asynchronous servers and clients that are very easy to use.
- PacketTcpServer that ensures reception without truncating the size of the data sent.
- HTTP server that is very simple to use.

## 📃 Usage

### Start Tcp Server
```csharp
RawTcpServer tcpServer = new RawTcpServer();
tcpServer.Start(5000);
```

### Can use Event Handlers
```csharp
RawTcpServer tcpServer = new RawTcpServer();
tcpServer.ClientConnected += ClientConnected;
tcpServer.ClientDisconnected += ClientDisconnected;
tcpServer.DataReceived += DataReceived;
```

### Get connected clinets
```csharp
tcpServer.Clients
```

### Send data to client
```csharp
tcpServer.Clients[0].Write(buffer, 0, buffer.Length);
```

### Send data to connected all clients
```csharp
tcpServer.WriteToAllClients(buffer, 0, buffer.Length);
```
---
### Connect to Tcp Server
```csharp
RawTcpClient tcpClient = new RawTcpClient();
tcpClient.Connect("127.0.0.1", 5000);
```

### Can use Event Handlers
```csharp
RawTcpClient tcpClient = new RawTcpClient();
tcpClient.Connected += Connected;
tcpClient.Disconnected += Disconnected;
tcpClient.DataReceived += DataReceived;
```

### Send data to server
```csharp
tcpClient.Write(buffer, 0, buffer.Length);
```
---
### Start Simple Http Server
See the example project for details.
```csharp
HttpServer httpServer = new HttpServer();
httpServer.HttpRequest += (sender, e) =>
{
    return new HttpResponse(HttpStatusCode.OK); 
};
httpServer.Start(); // default start port is 80
```

## 🐾 Examples

<details>
<summary style="font-Weight : bold; font-size : 30px; color : #E43914;" >Echo Server / Client</summary>
  
### Echo Server
```csharp
static void Main(string[] args)
{
    using (RawTcpServer tcpServer = new RawTcpServer())
    {
        tcpServer.ClientConnected += (sender, e) =>
            Console.WriteLine($"[{e}]: Connected"); // new client connected
        tcpServer.ClientDisconnected += (sender, e) =>
            Console.WriteLine($"[{e}]: Disconnected"); // client disconnected
        tcpServer.DataReceived += (sender, e) =>
        {
            byte[] readBytes = e.Client.ReadExisting(); // read all data
            string dataString = readBytes.Aggregate( // data to hex string
                new StringBuilder(32),
                (stringBuilder, data) => stringBuilder.Append($" 0x{data.ToString("X2")}")
                ).ToString().Trim();

            Console.WriteLine($"[{e.Client}]: {dataString}");

            e.Client.Write(readBytes, 0, readBytes.Length); // return same data
        };

        try
        {
            tcpServer.Start(5000);
            Console.WriteLine("Listening for connections...");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Console.ReadLine();
    }
}
```

### Echo Client
```csharp
static void Main(string[] args)
{
    using (RawTcpClient tcpClient = new RawTcpClient())
    {
        tcpClient.Connected += (sender, e) =>
            Console.WriteLine($"Connect to [{e.RemoteEndPoint}]");
        tcpClient.Disconnected += (sender, e) =>
            Console.WriteLine($"{Environment.NewLine}Disconnected from [{e.RemoteEndPoint}]");
        tcpClient.DataReceived += (sender, e) =>
        {
            if (sender is RawTcpClient rawTcpClient)
            {
                byte[] readBytes = rawTcpClient.ReadExisting(); // read all data
                Console.WriteLine($"DataReceived: {Encoding.ASCII.GetString(readBytes)}");
            }
        };

        try
        {
            tcpClient.Connect("127.0.0.1", 5000);

            while (true)
            {
                string line = Console.ReadLine();
                byte[] buffer = Encoding.ASCII.GetBytes(line);
                tcpClient.Write(buffer, 0, buffer.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }
}
```
  
</details>

<details>
<summary style="font-Weight : bold; font-size : 30px; color : #E43914;" >Packet Echo Server / Client</summary>
  
### Packet Echo Server
The size of the received data is received without being cut off.
```csharp
static void Main(string[] args)
{
  using (PacketTcpServer tcpServer = new PacketTcpServer())
  {
      tcpServer.ClientConnected += (sender, e) =>
          Console.WriteLine($"[{e}]: Connected"); // new client connected
      tcpServer.ClientDisconnected += (sender, e) =>
          Console.WriteLine($"[{e}]: Disconnected"); // client disconnected
      tcpServer.PacketReceived += (sender, e) =>
      {
          if (sender is PacketTcpServer packetTcpServer)
          {
              Console.WriteLine($"[{e.Packet.IPEndPoint}]: PacketReceived (PacketLength: {e.Packet.PacketData.Length})");
              packetTcpServer.WritePacket(e.Packet.TcpClient, e.Packet.PacketData); // return same packet
          }
      };

      try
      {
          tcpServer.Start(5000);
          Console.WriteLine("Listening for connections...");
      }
      catch (Exception ex)
      {
          Console.WriteLine(ex);
      }

      Console.ReadLine();
  }
}
```

### Packet Echo Client
```csharp
static void Main(string[] args)
{
    using (PacketTcpClient tcpClient = new PacketTcpClient())
    {
        tcpClient.Connected += (sender, e) =>
            Console.WriteLine($"Connect to [{e.RemoteEndPoint}]");
        tcpClient.Disconnected += (sender, e) =>
            Console.WriteLine($"{Environment.NewLine}Disconnected from [{e.RemoteEndPoint}]");
        tcpClient.PacketReceived += (sender, e) =>
        {
            Console.WriteLine($"PacketReceived: (PacketLength: {e.PacketData.Length})");
        };

        try
        {
            tcpClient.Connect("127.0.0.1", 5000);

            tcpClient.WritePacket(new byte[1024]); // send 1024 bytes
            tcpClient.WritePacket(new byte[1024 * 1024]); // send 1024 * 1024 bytes
            tcpClient.WritePacket(new byte[1024 * 1024 * 10]); // send 1024 * 1024 * 10 bytes

            Console.ReadLine();

            tcpClient.Disconnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        Console.ReadLine();
    }
}
```

![image](https://user-images.githubusercontent.com/49547202/130899797-381176ee-9ac8-4804-9c85-bd1e4300b4b5.png)
  
</details>

<details>
<summary style="font-Weight : bold; font-size : 30px; color : #E43914;" >Http Server</summary>
  
### Simple Http Server
```csharp
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
```

![image](https://user-images.githubusercontent.com/49547202/131252066-8d2bb113-48f7-4dfc-8ac2-039fe28c0de3.png)
  
</details>



## 🐞 Bug Report
If you find a bug, please report to us posting [issues](https://github.com/akon47/SimpleTcp/issues) on GitHub.
