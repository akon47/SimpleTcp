using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
    public class TextTcpClient : IDisposable
    {
        #region Properties
        public Encoding Encoding { get; private set; }
        #endregion

        #region Private Members
        private RawTcpClient _rawTcpClient;
        private bool _leaveOpen;
        #endregion

        public TextTcpClient(RawTcpClient rawTcpClient) : this(rawTcpClient, Encoding.UTF8) { }

        public TextTcpClient(RawTcpClient rawTcpClient, Encoding encoding, bool leaveOpen = false)
        {
            _rawTcpClient = rawTcpClient;
            _leaveOpen = leaveOpen;
            Encoding = encoding;
        }

        public virtual void Disconnect()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(!_leaveOpen)
                {
                    _rawTcpClient?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
