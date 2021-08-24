using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
	public abstract class BaseTcpServer : IDisposable
	{
		#region Properties
		public bool IsStarted { get => (tcpListener != null); }

		public TcpClient[] Clients
		{
			get
			{
				lock (clients)
				{
					return clients.Select(client => client.TcpClient).ToArray();
				}
			}
		}

		protected TcpListener TcpListener { get => tcpListener; }
		#endregion

		#region Private Member
		private TcpListener tcpListener = null;
		private List<Client> clients = new List<Client>();
		#endregion

		#region Public Methods
		public void Start(int port)
		{
			tcpListener = new TcpListener(IPAddress.Any, port);
			try
			{
				tcpListener.Start();
				tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), tcpListener);
			}
			catch(Exception e)
			{
				tcpListener?.Stop();
				tcpListener = null;
				throw e;
			}
		}

		public void Stop()
		{
			tcpListener?.Stop();
			tcpListener = null;

			lock(clients)
			{
				clients?.ForEach(client => client.TcpClient?.Client?.Disconnect(false));
				clients?.Clear();
			}
		}

		public virtual void Dispose()
		{
			Stop();
		}
		#endregion

		#region Protected Methods
		protected abstract void OnDataReceived(TcpClient tcpClient, byte[] data, int dataLength);
		protected abstract void OnClientConnected(TcpClient tcpClient);
		protected abstract void OnClientDisconnected(TcpClient tcpClient);
		#endregion

		#region Private Methods
		private void AcceptTcpClientCallback(IAsyncResult ar)
		{
			if(ar.AsyncState is TcpListener tcpListener)
			{
				TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
				if(tcpClient != null) // new client connected
				{
					Client client = new Client(tcpClient, tcpClient.ReceiveBufferSize);
					lock(clients)
					{
						clients.Add(client);
					}

					try
					{
						client.NetworkStream.BeginRead(client.Buffer, 0, client.BufferSize, new AsyncCallback(ReadCallback), client);
						OnClientConnected(client.TcpClient);
					}
					catch
					{
						if (tcpClient.Connected)
						{
							tcpClient.Close();
						}

						lock (clients)
						{
							clients.Remove(client);
						}
					}

					tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), this.tcpListener);
				}
			}
		}

		private void ReadCallback(IAsyncResult ar)
		{
			if (ar.AsyncState is Client client)
			{
				int readSize;
				NetworkStream networkStream = null;
				try
				{
					networkStream = client.NetworkStream;
					readSize = networkStream.EndRead(ar);
				}
				catch
				{
					readSize = 0;
				}

				if (readSize == 0) // client disconnected when readSize is zero
				{
					lock (clients)
					{
						if (clients.Contains(client))
						{
							clients.Remove(client);
						}
					}
					// disconnected
					OnClientDisconnected(client.TcpClient);
				}
				else
				{
					OnDataReceived(client.TcpClient, client.Buffer, readSize);

					networkStream?.BeginRead(client.Buffer, 0, client.BufferSize, new AsyncCallback(ReadCallback), client);
				}
			}
		}
		#endregion

		private class Client
		{
			public TcpClient TcpClient { get; private set; }
			public byte[] Buffer { get; private set; }
			public int BufferSize { get => Buffer?.Length ?? 0; }
			public NetworkStream NetworkStream { get => TcpClient?.GetStream(); }

			public Client(TcpClient tcpClient, int bufferSize)
			{
				TcpClient = tcpClient;
				Buffer = new byte[bufferSize];
			}
		}
	}
}
