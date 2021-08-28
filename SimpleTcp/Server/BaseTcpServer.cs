using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SimpleTcp.Utils;

namespace SimpleTcp.Server
{
    public abstract class BaseTcpServer : IDisposable
    {
        #region Properties
        /// <summary>
        /// Server is started
        /// </summary>
        public bool IsStarted { get => (tcpListener != null); }

        /// <summary>
        /// Connected clients
        /// </summary>
        public IClient[] Clients
        {
            get
            {
                lock (syncObject)
                {
                    return connections.ToArray();
                }
            }
        }

        /// <summary>
        /// Opened Server Port
        /// </summary>
        public int Port { get => (tcpListener?.LocalEndpoint as IPEndPoint)?.Port ?? -1; }

        /// <summary>
        /// Get Total received bytes count
        /// </summary>
        public long TotalReceivedBytes { get => totalReceivedBytes; }

        /// <summary>
        /// Get Total sended bytes count
        /// </summary>
        public long TotalSendedBytes { get => totalSendedBytes; }
        #endregion

            #region Private Member
        private object syncObject = new object();
        private TcpListener tcpListener = null;
        private List<Connection> connections = new List<Connection>();

        private long totalReceivedBytes = 0;
        private long totalSendedBytes = 0;
        #endregion

        #region Protected Member

        #endregion

        #region Public Member
        /// <summary>
        /// Client connected event handler
        /// </summary>
        public event ClientConnectedHandler ClientConnected;

        /// <summary>
        /// Client disconnected event handler
        /// </summary>
		public event ClientDisconnectedHandler ClientDisconnected;
        #endregion

        #region Public Methods
        #region Constructor

        /// <summary>
        /// BaseTcpServer
        /// </summary>
        /// <param name="port">If you specify a valid port, the server starts immediately.</param>
        public BaseTcpServer(int port = -1)
        {
            if (port > 0)
            {
                Start(port);
            }
        }
        #endregion

        /// <summary>
        /// Start tcp server
        /// </summary>
        /// <param name="port">Server port</param>
        public void Start(int port)
        {
            lock (syncObject)
            {
                if (IsStarted)
                {
                    throw new InvalidOperationException("already started");
                }

                tcpListener = new TcpListener(IPAddress.Any, port);
                try
                {
                    tcpListener.Start();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), tcpListener);
                }
                catch (Exception e)
                {
                    tcpListener?.Stop();
                    tcpListener = null;
                    throw e;
                }
            }
        }

        /// <summary>
        /// Stop tcp server
        /// </summary>
		public void Stop()
        {
            lock (syncObject)
            {
                try
                {
                    connections?.ForEach(client => client.TcpClient?.Client?.Disconnect(false));
                    connections?.Clear();
                }
                catch { }

                try
                {
                    tcpListener?.Stop();
                    tcpListener = null;
                }
                catch { }
            }
        }

        public virtual void Dispose()
        {
            Stop();
        }

        protected IClient GetClient(TcpClient tcpClient)
        {
            lock (syncObject)
            {
                return connections.First(connection => connection.TcpClient == tcpClient);
            }
        }

        protected void WriteToAllClients(byte[] buffer, int offset, int count)
        {
            Parallel.ForEach(connections, connection =>
            {
                connection.Write(buffer, offset, count);
            });
        }
        #endregion

        #region Protected Methods
        protected virtual void OnDataReceived(IClient client, int receivedSize) { }
        protected virtual void OnClientConnected(IClient client) { }
        protected virtual void OnClientDisconnected(IClient client) { }
        #endregion

        #region Private Methods
        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            if (ar.AsyncState is TcpListener tcpListener)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
                    if (tcpClient != null) // new client connected
                    {
                        Connection connection = new Connection(this, tcpClient);
                        lock (syncObject)
                        {
                            connections.Add(connection);
                        }

                        try
                        {
                            connection.BeginRead(
                                new Connection.DataReceivedCallback(DataReceivedCallback),
                                new Connection.DisconnectedCallback(DisconnectedCallback));
                            OnClientConnected(connection);
                            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(connection));
                        }
                        catch
                        {
                            if (tcpClient.Connected)
                            {
                                tcpClient.Close();
                            }

                            lock (syncObject)
                            {
                                connections.Remove(connection);
                            }
                        }

                        tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this.tcpListener);
                    }
                }
                catch { }
            }
        }

        private void DisconnectedCallback(Connection connection)
        {
            lock (syncObject)
            {
                if (connections.Contains(connection))
                {
                    connections.Remove(connection);
                }
            }
            OnClientDisconnected(connection);
            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(connection));

        }

        private void DataReceivedCallback(Connection connection, int receivedSize)
        {
            System.Threading.Interlocked.Add(ref totalReceivedBytes, receivedSize);
            OnDataReceived(connection, receivedSize);
        }
        #endregion

        protected class Connection : IClient
        {
            #region Properties
            public TcpClient TcpClient { get; private set; }
            public IPEndPoint IPEndPoint { get => TcpClient?.Client?.RemoteEndPoint as IPEndPoint; }

            public int BytesToRead { get => _ringBuffer.Count; }
            public long DropBytes { get; private set; } = 0;
            public long SendedBytes { get; private set; } = 0;
            public long ReceivedBytes { get; private set; } = 0;
            #endregion

            public delegate void DataReceivedCallback(Connection connection, int receivedSize);
            public delegate void DisconnectedCallback(Connection connection);

            #region Private Members
            private object syncObject = new object();
            private byte[] _buffer;
            private RingBuffer _ringBuffer;
            private DataReceivedCallback _dataReceived;
            private DisconnectedCallback _disconnected;
            private BaseTcpServer _baseTcpServer;
            #endregion

            public Connection(BaseTcpServer baseTcpServer, TcpClient tcpClient)
            {
                TcpClient = tcpClient;
                _buffer = new byte[tcpClient.ReceiveBufferSize];
                _ringBuffer = new RingBuffer(tcpClient.ReceiveBufferSize);
                _baseTcpServer = baseTcpServer;
            }

            public void BeginRead(DataReceivedCallback dataReceivedCallback, DisconnectedCallback disconnectedCallback)
            {
                _dataReceived = dataReceivedCallback;
                _disconnected = disconnectedCallback;
                try
                {
                    TcpClient?.GetStream()?.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(ReadCallback), this);
                }
                catch
                {
                    _disconnected?.Invoke(this);
                }
            }

            private void ReadCallback(IAsyncResult ar)
            {
                if (ar.AsyncState is Connection connection)
                {
                    int readSize;
                    try
                    {
                        readSize = connection.TcpClient.GetStream()?.EndRead(ar) ?? 0;
                    }
                    catch { readSize = 0; }

                    if (readSize == 0) // client disconnected when readSize is zero
                    {
                        _disconnected?.Invoke(this);
                    }
                    else
                    {
                        int writeBytes = _ringBuffer.Write(_buffer, 0, readSize);
                        if (writeBytes < readSize)
                        {
                            DropBytes += (readSize - writeBytes);
                        }
                        ReceivedBytes += readSize;
                        _dataReceived?.Invoke(this, readSize);
                        BeginRead(_dataReceived, _disconnected);
                    }
                }
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                return _ringBuffer.Read(buffer, offset, count);
            }

            public byte[] ReadExisting()
            {
                return _ringBuffer.ReadExisting();
            }

            public int ReadByte()
            {
                return _ringBuffer.ReadByte();
            }

            public void Write(byte[] buffer, int offset, int count)
            {
                try
                {
                    lock (syncObject)
                    {
                        NetworkStream networkStream = TcpClient?.GetStream();
                        if (networkStream.CanWrite)
                        {
                            networkStream.Write(buffer, offset, count);
                            networkStream.Flush();
                            SendedBytes += count;
                            System.Threading.Interlocked.Add(ref _baseTcpServer.totalSendedBytes, count);
                        }
                    }
                }
                catch
                {
                    _disconnected?.Invoke(this);
                }
            }

            public override string ToString()
            {
                return IPEndPoint?.ToString();
            }
        }
    }
}
