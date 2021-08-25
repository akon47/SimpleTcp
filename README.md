# SimpleTcp
A simple-to-use TCP server and client library.


# Examples

### Echo Server
```csharp
static void Main(string[] args)
{
    using (RawTcpServer tcpServer = new RawTcpServer())
    {
        tcpServer.ClientConnected += (sender, e) => Console.WriteLine($"[{e.Client}]: Connected"); // new client connected
        tcpServer.ClientDisconnected += (sender, e) => Console.WriteLine($"[{e.Client}]: Disconnected"); // client disconnected
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
        tcpClient.Connected += (sender, e) => Console.WriteLine($"Connect to [{e.RemoteEndPoint}]");
        tcpClient.Disconnected += (sender, e) => Console.WriteLine($"{Environment.NewLine}Disconnected from [{e.RemoteEndPoint}]");
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
