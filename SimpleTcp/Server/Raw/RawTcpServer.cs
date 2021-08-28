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
    public class RawTcpServer : BaseTcpServer
    {
        #region Public Member
        /// <summary>
        /// Called when data is received.
        /// </summary>
		public event DataReceivedEventHandler DataReceived;
        #endregion

        #region Public Methods

        #region Constructor
        /// <summary>
        /// RawTcpServer
        /// </summary>
        /// <param name="port">If you specify a valid port, the server starts immediately.</param>
        public RawTcpServer(int port = -1) : base(port) { }
        #endregion

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public void Write(TcpClient tcpClient, byte[] buffer, int offset, int count)
        {
            base.GetClient(tcpClient)?.Write(buffer, offset, count);
        }

        /// <summary>
        /// Send data to all connected clinets
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        new public void WriteToAllClients(byte[] buffer, int offset, int count)
        {
            base.WriteToAllClients(buffer, offset, count);
        }
        #endregion

        #region Protected Methods
        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(client));
        }
        #endregion
    }
}
