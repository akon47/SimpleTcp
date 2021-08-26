using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
    public class PacketTcpServer : BaseTcpServer
    {
        #region PrivateMember
        private object syncObject = new object();
        private Dictionary<TcpClient, Packet> packets = new Dictionary<TcpClient, Packet>();
        #endregion

        #region Public Member
        public event PacketReceivedEventHandler PacketReceived;
        #endregion

        #region Public Methods

        #region Constructor
        /// <summary>
        /// PacketTcpServer
        /// </summary>
        /// <param name="port">If you specify a valid port, the server starts immediately.</param>
        public PacketTcpServer(int port = -1) : base(port) { }
        #endregion

        #endregion

        #region Protected Methods
        protected override void OnDataReceived(IClient client, int receivedSize)
        {
            Packet packet = null;
            lock (syncObject)
            {
                if (!packets.ContainsKey(client.TcpClient))
                {
                    packet = new Packet(client.TcpClient);
                    packets.Add(client.TcpClient, packet);
                }
                else
                {
                    packet = packets[client.TcpClient];
                }
            }

            while (client.BytesToRead > 0)
            {
                packet.Write(client);

                if (packet.IsComplete)
                {
                    PacketReceived?.Invoke(this, new PacketReceivedEventArgs(packet));
                    lock (syncObject)
                    {
                        packet = new Packet(client.TcpClient);
                        packets[client.TcpClient] = packet;
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void WritePacket(TcpClient tcpClient, byte[] packetData)
        {
            IClient client = base.GetClient(tcpClient);
            if(client != null)
            {
                byte[] lengthBuffer = BitConverter.GetBytes(packetData.Length);
                client.Write(lengthBuffer, 0, lengthBuffer.Length);
                client.Write(packetData, 0, packetData.Length);
            }
            else
            {
                throw new KeyNotFoundException("tcpClient not found...");
            }
        }
        #endregion

        private class Packet : IPacket
        {
            #region Properties
            public TcpClient TcpClient { get; private set; }
			public IPEndPoint IPEndPoint { get => TcpClient?.Client?.RemoteEndPoint as IPEndPoint; }
            public byte[] PacketData { get => buffer; }
            public bool IsComplete { get => (buffer != null && (buffer.Length - writePosition) == 0); }
            #endregion

            #region Private Members
            private object syncObject = new object();
            private byte[] buffer;
            private int writePosition = 0;

            private byte[] lengthBuffer = new byte[4];
            private int lengthWritePosition = 0;
            #endregion

            public Packet(TcpClient tcpClient)
            {
                TcpClient = tcpClient;
            }

            public int Write(IClient client)
            {
                lock (syncObject)
                {
                    int bytesWritten = 0;

                    if (buffer == null)
                    {
                        int lengthRemain = 4 - lengthWritePosition;
                        int count = client.BytesToRead;
                        if (count >= lengthRemain)
                        {
                            bytesWritten = client.Read(lengthBuffer, 0, lengthRemain);
                            buffer = new byte[BitConverter.ToInt32(lengthBuffer, 0)];
                            lengthWritePosition += lengthRemain;
                        }
                        else
                        {
                            bytesWritten = client.Read(lengthBuffer, lengthWritePosition, count);
                            lengthWritePosition += bytesWritten;
                            return bytesWritten;
                        }
                    }

                    int remain = buffer.Length - writePosition;
                    int copiedBytes = client.Read(buffer, writePosition, remain);
                    bytesWritten += copiedBytes;
                    writePosition += copiedBytes;

                    return bytesWritten;
                }
            }

            public int Write(byte[] data, int offset, int count)
            {
                lock (syncObject)
                {
                    int bytesWritten = 0;

                    if(buffer == null)
                    {
                        int lengthRemain = 4 - lengthWritePosition;
                        if(count >= lengthRemain)
                        {
                            Array.Copy(data, offset, lengthBuffer, lengthWritePosition, lengthRemain);
                            buffer = new byte[BitConverter.ToUInt32(lengthBuffer, 0)];
                            lengthWritePosition += lengthRemain;
                            offset += lengthRemain;
                            count -= lengthRemain;
                            bytesWritten += lengthRemain;
                        }
                        else
                        {
                            Array.Copy(data, offset, lengthBuffer, lengthWritePosition, count);
                            lengthWritePosition += count;
                            return count;
                        }
                    }

                    int remain = buffer.Length - writePosition;
                    if (count > remain)
                    {
                        count = remain;
                    }

                    if (count > 0)
                    {
                        int writeToEnd = Math.Min(buffer.Length - writePosition, count);
                        Array.Copy(data, offset, buffer, writePosition, writeToEnd);
                        writePosition += writeToEnd;
                        bytesWritten += writeToEnd;
                        if (bytesWritten < count)
                        {
                            Array.Copy(data, offset + bytesWritten, buffer, writePosition, count - bytesWritten);
                            writePosition += (count - bytesWritten);
                            bytesWritten = count;
                        }
                    }

                    return bytesWritten;
                }
            }

            public byte[] GetBuffer()
            {
                return buffer;
            }
        }
    }
}
