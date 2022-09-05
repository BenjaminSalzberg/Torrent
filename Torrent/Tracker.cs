using System.Net.Sockets;
using System.Net;

namespace Torrent
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
        public string Name {get; set;} = "";
        // Announce: Name of the tracker (can be a list, can contain just 1 element)
        // also the url of the tracker

        //used primarily for announce list, prefer lower priority, defaults to 0 for announce
        public int priority {get; set; }
        public byte[]? trackerResponse {get; set; }
        public TrackerStatus status {get; set;} = TrackerStatus.Uncontacted; 
        public Protocols protocol {get; set; }
        public UdpClient? udpSocket {get; set; }
        //public long? connection_id {get; set;}
        public byte[]? connection_id {get; set;}
        // public int? transaction_id {get; set;}
        public byte[]? transaction_id {get; set;}
        //public wssSocket wssSocket {get; set; }
        //public HTMLSocket htmlSocket {get; set; }

        public Tracker(String name, int priority = 0)
        {
            this.Name = name;
            this.priority = priority;
        }
    } 
}