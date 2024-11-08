using System.Net.Sockets;
using System.Net;
using TorrentController.TorrentData;

namespace TorrentController.SocketHandling
{
	public enum Protocols
	{
		UDP,
		WSS,
		HTML
	}
	public class TrackerCommunication
	{
		public void SendTrackerCommunication(Tracker tracker, byte[] infoHash)
		{
			string announce = tracker.Name;
			Uri uri = new(announce);
			string protocol = uri.Scheme;
			string host = uri.Host;
			int port = uri.Port;

			//Console.WriteLine(announce);
			//Console.WriteLine(port);
			//Console.WriteLine(protocol);
			//Console.WriteLine(host);
			if (Enum.TryParse(protocol, true, out Protocols protocolEnum))
			{
				switch (protocolEnum)
				{
					case Protocols.UDP:
						//Console.WriteLine("UDP");
						try
						{
							tracker.Status = TrackerStatus.Contacted;
							UDPTracker(host, port, infoHash, tracker);
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
		private static void UDPTracker(string host, int port, byte[] info_hash, Tracker tracker)
		{
			// try 4 times once every 15 seconds, then stop retrying
			UdpClient udpTrackerSocket = new(51126);
			// set timeout to 15 seconds
			int TimeoutSeconds = 1;
			udpTrackerSocket.Client.ReceiveTimeout = TimeoutSeconds * 1000;
			long connection_id = 0x41727101980;
			int action = 0;
			Random random = new();
			int transaction_id = random.Next();

			byte[] connection_id_byte = BitConverter.GetBytes(connection_id);
			byte[] action_byte = BitConverter.GetBytes(action);
			byte[] transaction_id_byte = BitConverter.GetBytes(transaction_id);
			// set up the transaction_id 
			tracker.TransactionId ??= new byte[transaction_id_byte.Length];
			Array.Copy(transaction_id_byte, tracker.TransactionId, transaction_id_byte.Length);
			byte[] payload = new byte[connection_id_byte.Length + action_byte.Length + transaction_id_byte.Length];
			// Set proper Endian-ness. 
			Array.Reverse(connection_id_byte);
			Array.Reverse(action_byte);
			Array.Reverse(transaction_id_byte);
			// Copy the data into the sending packet
			Buffer.BlockCopy(connection_id_byte, 0, payload, 0, connection_id_byte.Length);
			Buffer.BlockCopy(action_byte, 0, payload, connection_id_byte.Length, action_byte.Length);
			Buffer.BlockCopy(transaction_id_byte, 0, payload, connection_id_byte.Length + action_byte.Length, transaction_id_byte.Length);
			// The number of retries for each test. 
			int maxRetries = 4;
			for (int i = 0; i < maxRetries; i++)
			{
				// try for a succes, it will fail at the .Receive function
				// if it succeeds, then we know that it has ben set to active. 
				try
				{
					// send the data
					udpTrackerSocket.Send(payload, connection_id_byte.Length + action_byte.Length + transaction_id_byte.Length, host, port);
					// set up the receive data endpoint
					IPEndPoint RemoteIpEndPoint = new(IPAddress.Any, 6969);
					// Receive the data
					byte[] receiveBytes = udpTrackerSocket.Receive(ref RemoteIpEndPoint);
					// preserve the tracker data, including the protocol, the socket, the received value, and the status of the tracker. 
					tracker.Protocol = Protocols.UDP;
					tracker.UdpSocket = udpTrackerSocket;
					tracker.TrackerResponse = receiveBytes;
					tracker.Status = TrackerStatus.Active;
					break;
				}
				catch (SocketException e) when (e.ErrorCode.Equals(10060))
				{
					continue;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					//throw e;
				}
			}
			if (tracker.Status != TrackerStatus.Active)
			{
				tracker.Status = TrackerStatus.Inactive;
				udpTrackerSocket.Close();
				udpTrackerSocket.Dispose();
				throw new NoResponseFromTrackerException();
			}
			else
			{
				Console.WriteLine("Tracker connect success");
				// Server response can be broken up into: 
				// int32_t  action          Describes the type of packet, in this case it should be 0, for connect. 3 for error
				// int32_t  transaction_id  Must match the transaction_id sent from the client.
				// int64_t	connection_id	A connection id, this is used when further information is exchanged with the tracker, to identify you. This connection id can be reused for multiple requests, but if it's cached for too long, it will not be valid anymore.
				var ResponseAction = tracker.TrackerResponse.AsSpan(0, 4).ToArray();
				var ResponseTransactionId = tracker.TrackerResponse.AsSpan(4, 4).ToArray();
				var ResponseConnectionId = tracker.TrackerResponse.AsSpan(8, 8).ToArray();
				// get local endienness  not network endienness
				Array.Reverse(ResponseAction);
				Array.Reverse(ResponseTransactionId);
				Array.Reverse(ResponseConnectionId);
				// ensure that the action is the correct action
				string ResponseActionString = BitConverter.ToString(ResponseAction).Replace("-", string.Empty);
				int ResponseActionValue = int.Parse(ResponseActionString, System.Globalization.NumberStyles.HexNumber);
				if (ResponseActionValue != 0)
				{
					throw new ActionErrorException(string.Format("The action value was not 0, and was {0}", ResponseActionString));
				}
				// ensure that the transaction ids match
				if (tracker.TransactionId.SequenceEqual(ResponseTransactionId))
				{
					//Console.WriteLine("Transaction Id's match");
				}
				else
				{
					throw new TransactionIdMismatchException(string.Format("The transaction id of the server is {0}, and the transaction of the client is {}", BitConverter.ToString(ResponseTransactionId), BitConverter.ToString(transaction_id_byte)));
				}
				// We now set the tracker's connection id to the response from the server. We no longer need to identify the 
				tracker.ConnectionId = ResponseConnectionId;
			}
		}

		public class NoResponseFromTrackerException : Exception
		{
			public NoResponseFromTrackerException()
			{ }
			public NoResponseFromTrackerException(string message) : base(message)
			{ }
			public NoResponseFromTrackerException(string message, Exception inner) : base(message, inner)
			{ }
		}

		public class TransactionIdMismatchException : Exception
		{
			public TransactionIdMismatchException()
			{ }
			public TransactionIdMismatchException(string message) : base(message)
			{ }
			public TransactionIdMismatchException(string message, Exception inner) : base(message, inner)
			{ }
		}

		public class ActionErrorException : Exception
		{
			public ActionErrorException()
			{ }
			public ActionErrorException(string message) : base(message)
			{ }
			public ActionErrorException(string message, Exception inner) : base(message, inner)
			{ }
		}

		public static async void SendWebRequest(string url)
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
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
	}


}