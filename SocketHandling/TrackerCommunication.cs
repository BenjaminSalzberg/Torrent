using System.Net.Sockets;
using System.Net;

namespace Torrent
{
    public enum Protocols
        {
            UDP, 
            WSS, 
            HTML
        }
    public class TrackerCommunication
    {
        public void SendTrackerCommunication(Tracker tracker, byte[] info_hash)
        {
            string announce = tracker.Name;
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
                        try
                        {
                            tracker.status = TrackerStatus.Contacted;
                            UDPTracker(host, port, info_hash, tracker);
                        }
                        catch (NoResponseFromTrackerException)
                        {
                            Console.WriteLine("No response from " + tracker.Name);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    case Protocols.WSS:
                        Console.WriteLine("WSS");
                        throw new Exception("This protocol has not been implemented");
                        //break;
                    case Protocols.HTML:
                        Console.WriteLine("html");
                        throw new Exception("This protocol has not been implemented");
                        //break;
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
        private void UDPTracker(String host, int port, byte[] info_hash, Tracker tracker)
        {
            // try 4 times once every 15 seconds, then stop retrying
            UdpClient udpTrackerSocket = new UdpClient(51126);
            // set timeout to 15 seconds
            int TimeoutSeconds = 1;
            udpTrackerSocket.Client.ReceiveTimeout = (TimeoutSeconds * 1000);
            long connection_id = 0x41727101980;
            int action = 0;
            Random random = new Random();
            int transaction_id = random.Next();

            byte[] connection_id_byte = BitConverter.GetBytes(connection_id);
            byte[] action_byte = BitConverter.GetBytes(action);
            byte[] transaction_id_byte = BitConverter.GetBytes(transaction_id);
            byte[] payload = new byte[connection_id_byte.Length + action_byte.Length + transaction_id_byte.Length];
            // Set proper Endian-ness. 
            Array.Reverse(connection_id_byte);
            Array.Reverse(action_byte);
            Array.Reverse(transaction_id_byte);
            Buffer.BlockCopy(connection_id_byte, 0, payload, 0, connection_id_byte.Length);
            Buffer.BlockCopy(action_byte, 0, payload, connection_id_byte.Length, action_byte.Length);
            Buffer.BlockCopy(transaction_id_byte, 0, payload, connection_id_byte.Length+action_byte.Length, transaction_id_byte.Length);
            // The number of retries for each test. 
            int maxRetries = 4;
            for(int i=0; i < maxRetries; i++)
            {
                // try for a succes, it will fail at the .Receive function
                // if it succeeds, then we know that it has ben set to active. 
                try{
                    Console.WriteLine("Sending packet");
                    udpTrackerSocket.Send(payload, connection_id_byte.Length + action_byte.Length + transaction_id_byte.Length, host, port);
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 6969);
                    Byte[] receiveBytes = udpTrackerSocket.Receive(ref RemoteIpEndPoint);
                    string returnData = System.Text.Encoding.ASCII.GetString(receiveBytes);
                    Console.WriteLine("This is the message you received " + returnData.ToString());
                    tracker.protocol = Protocols.UDP;
                    tracker.udpSocket = udpTrackerSocket;
                    tracker.trackerResponse = receiveBytes;
                    tracker.transaction_id = transaction_id;
                    // A connection id, this is used when further information is exchanged with the tracker, to identify you. This connection id can be reused for multiple requests, but if it's cached for too long, it will not be valid anymore.
                    tracker.connection_id = connection_id;
                    tracker.status = TrackerStatus.Active;
                    break;
                    //return tracker;
                }
                catch (SocketException e) when (e.ErrorCode.Equals(10060))
                {
                    continue;
                }
                catch (Exception e ) {
                    Console.WriteLine(e.ToString());
                    throw e;
                }
            }
            if(tracker.status != TrackerStatus.Active)
            {
                tracker.status = TrackerStatus.Inactive;
                udpTrackerSocket.Close();
                udpTrackerSocket.Dispose();
                throw new NoResponseFromTrackerException();
            }
            else
            {
                Console.WriteLine("Tracker connect success");
            }            
        }

        public class NoResponseFromTrackerException : Exception
        {
            public NoResponseFromTrackerException()
            {}
            public NoResponseFromTrackerException(string message) : base(message)
            {}
            public NoResponseFromTrackerException(string message, Exception inner) : base(message, inner)
            {}
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