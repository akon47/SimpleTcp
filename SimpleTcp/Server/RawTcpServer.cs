﻿using System;
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
		public event ClientConnectedHandler ClientConnected;
		public event ClientDisconnectedHandler ClientDisconnected;
		public event DataReceivedEventHandler DataReceived;
        #endregion

        #region Protected Methods
        protected override void OnClientConnected(IClient client)
        {
            ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client));
        }

        protected override void OnClientDisconnected(IClient client)
        {
            ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(client));
        }

        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(client));
        }
        #endregion
    }
}
