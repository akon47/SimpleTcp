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
	public abstract class BaseTcpClient : IDisposable
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

        #region Private Member
        private object syncObject = new object();
        private TcpClient tcpClient;
        private byte[] buffer;
        private RingBuffer ringBuffer;
        #endregion

        #region Public Methods
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
        #endregion

        #region Protected Methods
		protected virtual void OnDataReceived(TcpClient tcpClient, int receivedSize) { }
		protected virtual void OnConnected(TcpClient tcpClient) { }
		protected virtual void OnDisconnected(TcpClient tcpClient) { }
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
                        OnConnected(tcpClient);

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
                    OnDisconnected(tcpClient);
                }
                else
                {
                    int writeBytes = ringBuffer.Write(buffer, 0, readSize);
                    if (writeBytes < readSize)
                    {
                        DropBytes += (readSize - writeBytes);
                    }

                    OnDataReceived(tcpClient, readSize);
                    networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), tcpClient);
                }
            }
        }
        #endregion
	}
}
