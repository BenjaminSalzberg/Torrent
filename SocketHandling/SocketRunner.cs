using System.Net.Sockets;
using System.Net;


namespace Torrent
{

    public class SocketRunner
    {
        public async void sendUDPRequest()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            

        }
    }

    
}