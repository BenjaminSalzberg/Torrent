using System.Net.Sockets;
using System.Net;
using TorrentController.SocketHandling;

namespace TorrentController.TorrentData
{
	public enum TrackerStatus
	{
		Inactive,
		Active,
		Uncontacted,
		Contacted
	}
	public class Tracker
	{
		// Name of the tracker 
		public string Name { get; set; } = "";
		// Announce: Name of the tracker (can be a list, can contain just 1 element)
		// also the url of the tracker

		//used primarily for announce list, prefer lower priority, defaults to 0 for announce
		public int Priority { get; set; }
		public byte[]? TrackerResponse { get; set; }
		public TrackerStatus Status { get; set; } = TrackerStatus.Uncontacted;
		public Protocols Protocol { get; set; }
		public UdpClient? UdpSocket { get; set; }
		//public long? connection_id {get; set;}
		public byte[]? ConnectionId { get; set; }
		// public int? transaction_id {get; set;}
		public byte[]? TransactionId { get; set; }
		//public wssSocket wssSocket {get; set; }
		//public HTMLSocket htmlSocket {get; set; }

		public Tracker(string name, int priority = 0)
		{
			this.Name = name;
			this.Priority = priority;
		}
	}
}