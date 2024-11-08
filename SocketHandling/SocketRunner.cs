using System.Net.Sockets;
using System.Net;


namespace TorrentController.SocketHandling
{
	public class SocketRunner
	{
		public static void SendUDPRequest()
		{
			Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


		}
	}


}