using System.Net.Sockets;
using System.Net;


namespace Torrent
{

    public class TrackerCommunication
    {

        public enum Protocols
        {
            UDP, 
            WSS, 
            HTML
        }
        public async void SendTrackerCommunication(String announce, byte[] info_hash)
        {
            Uri uri = new Uri(announce);
            String protocol = uri.Scheme;
            String host = uri.Host;
            int port = (int)(uri.Port);

            //Console.WriteLine(announce);
            //Console.WriteLine(port);
            //Console.WriteLine(protocol);
            //Console.WriteLine(host);
            if(Enum.TryParse(protocol, true, out Protocols protocolEnum))
            {
                switch(protocolEnum)
                {
                    case Protocols.UDP:
                        //Console.WriteLine("UDP");
                        UDPTracker(host, port, info_hash);
                        break;
                    case Protocols.WSS:
                        //Console.WriteLine("WSS");
                        break;
                    case Protocols.HTML:
                        //Console.WriteLine("html");
                        break;
                    default:
                        // This should be unreachable
                        throw new Exception("This protocol has not been implemented");
                }
            }
            else
            {
                throw new Exception("This protocol has not been implemented");
            }
        }
        private async void UDPTracker(String host, int port, byte[] info_hash)
        {
            UdpClient udpTrackerSocket = new UdpClient(6969);
            try{
                //udpTrackerSocket.Connect(host, port);
                udpTrackerSocket.Send(info_hash, info_hash.Count(), host, port);
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                Byte[] receiveBytes = udpTrackerSocket.Receive(ref RemoteIpEndPoint);
                string returnData = System.Text.Encoding.ASCII.GetString(receiveBytes);
                Console.WriteLine("This is the message you received " + returnData.ToString());
            }
            catch (Exception e ) {
                Console.WriteLine(e.ToString());
            }

            
        }

        public async void sendWebRequest(String url)
        {
            try	
            {
                HttpResponseMessage response = await Program.client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    }

    
}