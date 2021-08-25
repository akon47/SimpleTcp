using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
    public class TextTcpServer : IDisposable
    {
        #region Properties
        public Encoding Encoding { get; private set; }
        #endregion

        #region Private Members
        private RawTcpServer _rawTcpServer;
        private bool _leaveOpen;
        #endregion

        #region Public Members
        public event TextReceivedEventHandler TextReceived;
        #endregion

        #region Public Methods

        #region Constructor
        public TextTcpServer(RawTcpServer rawTcpServer) : this(rawTcpServer, Encoding.UTF8) { }

        public TextTcpServer(RawTcpServer rawTcpServer, Encoding encoding, bool leaveOpen = false)
        {
            if (rawTcpServer == null)
                throw new NullReferenceException("rawTcpServer is null");

            _rawTcpServer = rawTcpServer;
            _leaveOpen = leaveOpen;
            Encoding = encoding;

            rawTcpServer.DataReceived += RawTcpServer_DataReceived;
        }
        #endregion

        public virtual void Stop()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Protected Methods
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                _rawTcpServer.DataReceived -= RawTcpServer_DataReceived;
                if(!_leaveOpen)
                {
                    _rawTcpServer?.Dispose();
                }
            }
        }
        #endregion

        #region Private Methods
        private void RawTcpServer_DataReceived(object sender, DataReceivedEventArgs e)
        {
            
        }
        #endregion
    }
}
