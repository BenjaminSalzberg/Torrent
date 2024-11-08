using TorrentController.BEncoding;
using TorrentController.SocketHandling;
using TorrentController.TorrentData;

namespace TorrentController
{
	class Program
	{
		public static readonly HttpClient client = new();
		public static void Main(string[] args)
		{
			string path = "D:\\Users\\Benjamin\\Downloads\\tears-of-steel.torrent";
			string name = "tears-of-steel.torrent";
			Dictionary<string, object> obj = (Dictionary<string, object>)BDecode.Decode(path);
			Torrent torrentDictionary = BencodeToTorrent(obj, name);
			//String announce = System.Text.Encoding.UTF8.GetString((byte[])obj["announce"]);
			//print(String.Join("\n", obj.Keys.ToArray()));

			//print(String.Join("\n", ((Dictionary<String, Object>)obj["info"]).Keys.ToArray()));
			//print(announce);
			//List<Tracker> trackLst = torrentDictionary.sortedTrackerLst();
			//foreach (var item in trackLst)
			//{
			//    print(item.Name);
			//    print(item.priority);
			//}
			//SocketRunner sockets = new SocketRunner();
			//print(torrentDictionary.info_hash);
			TrackerCommunication communication = new();
			foreach (Tracker tracker in torrentDictionary.TrackerList)
			{
				string announce = tracker.Name;
				Print("Starting tracker " + announce);
				try
				{
					communication.SendTrackerCommunication(tracker, torrentDictionary.InfoHash);
				}
				catch (Exception e) when (e.Message == "This protocol has not been implemented")
				{
					Print(e);
					continue;
				}
				catch (Exception e)
				{
					Print(e);
				}
				if (tracker.TrackerResponse != null)
				{
					//Console.WriteLine(BitConverter.ToString(tracker.trackerResponse));
					break;
				}
			}
			var activeTrackers = torrentDictionary.TrackerList.Where(x => x.Status == TrackerStatus.Active);
			foreach (var item in activeTrackers)
			{
				Print(item.Name);
				if (item.TransactionId != null)
					Print(BitConverter.ToString(item.TransactionId));
				if (item.ConnectionId != null)
					Print(BitConverter.ToString(item.ConnectionId));
			}
			//communication.sendWebRequest(announce);
		}

		private static Torrent BencodeToTorrent(Dictionary<string, object> dict, string name)
		{
			Torrent torrent = new(dict, name);
			return torrent;
		}
		public static void Print(object str)
		{
			Console.WriteLine(str);
		}
	}
}