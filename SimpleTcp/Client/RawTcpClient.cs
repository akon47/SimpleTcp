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
        /// <summary>
        /// Called when data is received.
        /// </summary>
        public event DataReceivedEventHandler DataReceived;
        #endregion

        #region Public Methods
        #region Constructor

        /// <summary>
        /// RawTcpClient
        /// </summary>
        /// <param name="host">The name of the remote host</param>
        /// <param name="port">The port number of the remote host</param>
        /// <param name="timeout">connection timeout (ms)</param>
        public RawTcpClient(string host = null, int port = -1, int timeout = 3000) : base(host, port, timeout) { }
        #endregion

        new public int Read(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, count);
        }

        new public byte[] ReadExisting()
        {
            return base.ReadExisting();
        }

        /// <summary>
        /// If exist return readed byte
        /// If not exist return -1
        /// </summary>
        /// <returns></returns>
        new public int ReadByte()
        {
            return base.ReadByte();
        }

        new public void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
        }
        #endregion

        #region Protected Methods
        protected override void OnDataReceived(TcpClient tcpClient, int receivedSize)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(receivedSize));
        }
        #endregion
    }
}
