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
		public event DataReceivedEventHandler DataReceived;
        #endregion

        #region Public Methods

        #region Constructor
        public RawTcpServer(int port = -1)
        {
            if(port > 0)
            {
                base.Start(port);
            }
        }
        #endregion

        #endregion

        #region Protected Methods
        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(client));
        }
        #endregion
    }
}
