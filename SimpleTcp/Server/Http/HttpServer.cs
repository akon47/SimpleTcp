using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpServer : BaseTcpServer
    {
        #region Public Member
        /// <summary>
        /// Called when request from client.
        /// </summary>
		public event HttpRequestEventHandler HttpRequest;
        #endregion

        #region Public Methods

        #region Constructor
        /// <summary>
        /// RawTcpServer
        /// </summary>
        /// <param name="port">If you specify a valid port, the server starts immediately.</param>
        public HttpServer(int port = -1) : base(port) { }
        #endregion

        public void Start()
        {
            base.Start(80);
        }
        #endregion

        #region Protected Methods
        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            
        }
        #endregion
    }
}
