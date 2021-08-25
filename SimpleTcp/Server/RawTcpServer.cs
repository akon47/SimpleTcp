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
	public class RawTcpServer : IDisposable
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

        public long TotalReceiveBytes { get; private set; }
		#endregion

		#region Private Member
		private object syncObject = new object();
		private TcpListener tcpListener = null;
		private List<Connection> connections = new List<Connection>();
		#endregion

		#region Protected Member
		
		#endregion

		#region Public Member
		public event ClientConnectedHandler ClientConnected;
		public event ClientDisconnectedHandler ClientDisconnected;
		public event DataReceivedEventHandler DataReceived;
        #endregion

        #region Public Methods

        #region Constructor
        public RawTcpServer(int port = -1)
        {
            if(port > 0)
            {
                Start(port);
            }
        }
        #endregion

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

		public void Stop()
		{
            lock (syncObject)
            {
                tcpListener?.Stop();
                tcpListener = null;


                connections?.ForEach(client => client.TcpClient?.Client?.Disconnect(false));
                connections?.Clear();
            }
		}

		public virtual void Dispose()
		{
			Stop();
		}

		public IClient GetClient(TcpClient tcpClient)
		{
			lock(syncObject)
			{
				return connections.First(connection => connection.TcpClient == tcpClient);
			}
		}

		public void WriteToAllClients(byte[] buffer, int offset, int count)
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
			if(ar.AsyncState is TcpListener tcpListener)
			{
				TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
				if(tcpClient != null) // new client connected
				{
					Connection connection = new Connection(tcpClient);
					lock(syncObject)
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
			OnDataReceived(connection, receivedSize);
			DataReceived?.Invoke(this, new DataReceivedEventArgs(connection));
            TotalReceiveBytes += receivedSize;
		}
		#endregion
		

		private class Connection : IClient
		{
			#region Properties
			public TcpClient TcpClient { get; private set; }
			public IPEndPoint IPEndPoint { get => TcpClient?.Client?.RemoteEndPoint as IPEndPoint; }
			public NetworkStream NetworkStream { get => TcpClient?.GetStream(); }

			public int BytesToRead { get => ringBuffer.Count; }
			public long DropBytes { get; private set; } = 0;
            public long SendBytes { get; private set; } = 0;
            public long ReceiveBytes { get; private set; } = 0;
			#endregion

			public delegate void DataReceivedCallback(Connection connection, int receivedSize);
			public delegate void DisconnectedCallback(Connection connection);

			#region Private Members
			private object syncObject = new object();
			private byte[] buffer;
			private RingBuffer ringBuffer;
			private DataReceivedCallback dataReceived;
			private DisconnectedCallback disconnected;
			#endregion

			public Connection(TcpClient tcpClient)
			{
				TcpClient = tcpClient;
				buffer = new byte[tcpClient.ReceiveBufferSize];
				ringBuffer = new RingBuffer(tcpClient.ReceiveBufferSize);
			}

			public void BeginRead(DataReceivedCallback dataReceivedCallback, DisconnectedCallback disconnectedCallback)
			{
				dataReceived = dataReceivedCallback;
				disconnected = disconnectedCallback;
				NetworkStream?.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), this);
			}

			private void ReadCallback(IAsyncResult ar)
			{
				if (ar.AsyncState is Connection connection)
				{
					int readSize = 0;
					try
					{
						readSize = connection.NetworkStream?.EndRead(ar) ?? 0;
					}
					catch { }

					if (readSize == 0) // client disconnected when readSize is zero
					{
						disconnected?.Invoke(this);
					}
					else
					{
						int writeBytes = ringBuffer.Write(buffer, 0, readSize);
						if(writeBytes < readSize)
						{
							DropBytes += (readSize - writeBytes);
						}
                        ReceiveBytes += readSize;

						dataReceived?.Invoke(this, readSize);
						BeginRead(dataReceived, disconnected);
					}
				}
			}

			public int Read(byte[] buffer, int offset, int count)
			{
				return ringBuffer.Read(buffer, offset, count);
			}

			public byte[] ReadExisting()
			{
                return ringBuffer.ReadExisting();
			}

			public int ReadByte()
			{
				return ringBuffer.ReadByte();
			}

			public void Write(byte[] buffer, int offset, int count)
			{
				lock(syncObject)
				{
					NetworkStream networkStream = NetworkStream;
					if(networkStream.CanWrite)
					{
						networkStream.Write(buffer, offset, count);
                        networkStream.Flush();
                        SendBytes += count;
					}
				}
			}

			public override string ToString()
			{
				return IPEndPoint?.ToString();
			}
		}
	}
}
