using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Client
{
    public class PacketTcpClient : BaseTcpClient
    {
        #region Private Members
        private object syncObject = new object();
        private byte[] packetData;
        private int writePosition = 0;

        private byte[] lengthBuffer = new byte[4];
        private int lengthWritePosition = 0;
        #endregion

        #region Public Member
        public event PacketReceivedEventHandler PacketReceived;
        #endregion

        #region Public Methods
        #region Constructor
        public PacketTcpClient(string host = null, int port = -1, int timeout = 3000) : base(host, port, timeout) { }
        #endregion

        public void WritePacket(byte[] packetData)
        {
            byte[] lengthBuffer = BitConverter.GetBytes(packetData.Length);
            base.Write(lengthBuffer, 0, lengthBuffer.Length);
            base.Write(packetData, 0, packetData.Length);
        }
        #endregion

        #region Protected Methods
        protected override void OnDataReceived(TcpClient tcpClient, int receivedSize)
        {
            lock (syncObject)
            {
                while (base.BytesToRead > 0)
                {
                    if (packetData == null)
                    {
                        int lengthRemain = 4 - lengthWritePosition;
                        int count = base.BytesToRead;
                        if (count >= lengthRemain)
                        {
                            lengthWritePosition += base.Read(lengthBuffer, 0, lengthRemain);
                            packetData = new byte[BitConverter.ToUInt32(lengthBuffer, 0)];
                        }
                        else
                        {
                            lengthWritePosition += base.Read(lengthBuffer, lengthWritePosition, count);
                            return;
                        }
                    }

                    int remain = packetData.Length - writePosition;
                    writePosition += base.Read(packetData, writePosition, remain);

                    if (packetData.Length - writePosition == 0)
                    {
                        PacketReceived?.Invoke(this, new PacketReceivedEventArgs(packetData));

                        packetData = null;
                        writePosition = 0;
                        lengthWritePosition = 0;
                    }
                }
            }
        }
        #endregion
    }
}
