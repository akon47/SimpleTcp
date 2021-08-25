using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleTcp.Utils;

namespace SimpleTcp.Client
{
	public class RawTcpClient : IDisposable
	{
        #region Properties
		/// <summary>
        /// Is Connected
        /// </summary>
		public bool IsConnected { get => (tcpClient?.Client?.Connected ?? false); }

        /// <summary>
        /// BytesToRead
        /// </summary>
        public int BytesToRead { get => ringBuffer.Count; }

        /// <summary>
        /// DropBytes
        /// </summary>
		public int DropBytes { get; private set; } = 0;
        #endregion

        #region Public Member
		public event ConnectedHandler Connected;
		public event DisconnectedHandler Disconnected;
		public event DataReceivedEventHandler DataReceived;
		#endregion

        #region Private Member
        private object syncObject = new object();
        private TcpClient tcpClient;
        private byte[] buffer;
        private RingBuffer ringBuffer;
        #endregion


        #region Public Methods
        #region Constructor
        public RawTcpClient(string host = null, int port = -1, int timeout = 3000)
        {
            if(string.IsNullOrWhiteSpace(host) && port > 0)
            {
                Connect(host, port, timeout);
            }
        }
        #endregion

        public void Connect(string host, int port, int timeout = 3000)
        {
            lock (syncObject)
            {
                if (IsConnected)
                {
                    throw new InvalidOperationException("already connected");
                }

                DropBytes = 0;
                tcpClient = new TcpClient();
                IAsyncResult ar = tcpClient.BeginConnect(host, port, new AsyncCallback(ConnectCallback), tcpClient);
                WaitHandle waitHandle = ar?.AsyncWaitHandle;
                try
                {
                    if (!(waitHandle?.WaitOne(timeout, false) ?? false) || !(tcpClient?.Connected ?? false))
                    {
                        tcpClient.Close();
                        tcpClient = null;
                        throw new TimeoutException("connection timeout");
                    }
                }
                finally
                {
                    waitHandle?.Close();
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

        public virtual void Dispose()
		{
            Disconnect();
		}

        public void Write(byte[] buffer, int offset, int count)
        {
            lock (syncObject)
            {
                NetworkStream networkStream = tcpClient?.GetStream();
                if (networkStream.CanWrite)
                {
                    networkStream.Write(buffer, offset, count);
                }
            }
        }
        #endregion

        #region Private Methods
        private void ConnectCallback(IAsyncResult ar)
        {
            lock (syncObject)
            {
                if (ar?.AsyncState is TcpClient tcpClient)
                {
                    if (tcpClient.Client?.Connected ?? false)
                    {
                        Connected?.Invoke(this, new ConnectedEventArgs(tcpClient));

                        buffer = new byte[tcpClient.ReceiveBufferSize];
                        ringBuffer = new RingBuffer(tcpClient.ReceiveBufferSize);

                        NetworkStream networkStream = tcpClient.GetStream();
                        networkStream?.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), tcpClient);
                    }
                }
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            if (ar?.AsyncState is TcpClient tcpClient)
            {
                NetworkStream networkStream = tcpClient.GetStream();

                int readSize = 0;
                try
                {
                    readSize = networkStream?.EndRead(ar) ?? 0;
                }
                catch { }

                if (readSize == 0) // disconnected when readSize is zero
                {
                    Disconnected?.Invoke(this, new DisconnectedEventArgs(tcpClient));
                }
                else
                {
                    int writeBytes = ringBuffer.Write(buffer, 0, readSize);
                    if (writeBytes < readSize)
                    {
                        DropBytes += (readSize - writeBytes);
                    }

                    DataReceived?.Invoke(this, new DataReceivedEventArgs(readSize));
                    networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), tcpClient);
                }
            }
        }
        #endregion

        public void Disconnect()
        {
            lock(syncObject)
            {
                tcpClient?.Close();
                tcpClient = null;

                buffer = null;
                ringBuffer = null;
            }
        }
	}
}
