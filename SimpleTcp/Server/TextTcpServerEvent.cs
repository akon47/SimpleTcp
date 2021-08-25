using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcp.Server
{
    public class TextReceivedEventArgs : EventArgs
	{
		public TcpClient TcpClient { get; }
		public IPEndPoint IPEndPoint { get; }
        public string Text { get; private set; }

		public TextReceivedEventArgs(IClient client, string text)
		{
			TcpClient = client.TcpClient;
            IPEndPoint = client.IPEndPoint;
            Text = text;
		}
	}
	public delegate void TextReceivedEventHandler(object sender, TextReceivedEventArgs e);
}
