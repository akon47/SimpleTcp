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
	public class RawTcpClient : BaseTcpClient
	{
        #region Public Member
		public event ConnectedHandler Connected;
		public event DisconnectedHandler Disconnected;
		public event DataReceivedEventHandler DataReceived;
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
        #endregion

        #region Protected Methods
        protected override void OnConnected(TcpClient tcpClient)
        {
            Connected?.Invoke(this, new ConnectedEventArgs(tcpClient));
        }

        protected override void OnDisconnected(TcpClient tcpClient)
        {
            Disconnected?.Invoke(this, new DisconnectedEventArgs(tcpClient));
        }

        protected override void OnDataReceived(TcpClient tcpClient, int receivedSize)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(receivedSize));
        }
        #endregion
    }
}
