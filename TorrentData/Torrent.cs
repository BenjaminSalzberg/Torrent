using System.Security.Cryptography;
using TorrentController.BEncoding;
namespace TorrentController.TorrentData
{
	public class Torrent
	{
		// Name of the torrent 
		public string Name { get; private set; }
		// Announce: Name of the tracker (can be a list, can contain just 1 element)
		public List<Tracker> TrackerList { get; private set; } = new List<Tracker>();
		// Creation Date
		public DateTime CreationDate { get; private set; }
		// Comment
		public string Comment = "";
		// Created By
		public string CreatedBy = "";
		// The string encoding format used to generate the pieces part of the info dictionary in the .torrent metafile
		public TorrentEncoding TorrentEncoding { get; private set; }
		// the 
		public InfoDictionary Infodictionary { get; private set; }

		public byte[] InfoHash { get; private set; }

		// Torrent Constructor
		public Torrent(Dictionary<string, object> dict, string name)
		{
			//name of the torrent file
			this.Name = name;
			// convert to unix timestamp
			if (dict.TryGetValue("creation date", out object? creationDate))
				this.CreationDate = DateTimeOffset.FromUnixTimeSeconds((long)creationDate).DateTime;
			if (dict.TryGetValue("created by", out object? createdBy))
				this.CreatedBy = System.Text.Encoding.UTF8.GetString((byte[])createdBy);
			if (dict.TryGetValue("comment", out object? comment))
				this.Comment = System.Text.Encoding.UTF8.GetString((byte[])comment);

			// tracker list generate using announce and tracker list
			string announce = System.Text.Encoding.UTF8.GetString((byte[])dict["announce"]);
			List<object> AnnounceLst;
			if (dict.TryGetValue("announce-list", out object? announceList))
			{
				AnnounceLst = (List<object>)announceList;
				this.TrackerList = BencodeToTrackerLst(AnnounceLst);
			}
			else
			{
				this.TrackerList = new List<Tracker>();
				TrackerList.Add(new Tracker(announce));
			}

			//BEncode.Encode(dict["info"]);
			this.InfoHash = GetInfoHash(BEncode.Encode(dict["info"]));
			this.TorrentEncoding = new TorrentEncoding(System.Text.Encoding.UTF8.GetString((byte[])dict["encoding"]));

			Dictionary<string, object> infodict = (Dictionary<string, object>)dict["info"];
			this.Infodictionary = BencodeToInfoDict(infodict);

			// This is url-list it is not implemented here, it is used to tell you where torrent data may be retrieved. 
			//List<Object> URLLst = (List<Object>)dict["url-list"];
			//Console.WriteLine(URLLst.Count);
			//Console.WriteLine(System.Text.Encoding.UTF8.GetString((byte[])URLLst[0]));
		}
		private static byte[] GetInfoHash(byte[] infodict)
		{
			byte[] hashBytes;
			hashBytes = SHA1.HashData(infodict);
			//string hash = BitConverter.ToString(hashBytes).Replace("-",String.Empty);
			//Console.WriteLine("The SHA1 hash is: " + hash);
			return hashBytes;
		}

		public List<Tracker> SortedTrackerLst()
		{
			List<Tracker> retLst = new(TrackerList);
			retLst.Sort(new Comparison<Tracker>((x, y) => x.Priority.CompareTo(y.Priority)));
			return retLst;
		}
		private static List<Tracker> BencodeToTrackerLst(List<object> AnnounceLst)
		{
			List<Tracker> trackerlst = new();
			for (int i = 0; i < AnnounceLst.Count; i++)
			{
				List<object> sublst = (List<object>)AnnounceLst[i];
				foreach (var item in sublst)
				{
					Tracker track = new(System.Text.Encoding.UTF8.GetString((byte[])item), i);
					trackerlst.Add(track);
				}
			}
			return trackerlst;
		}
		private static InfoDictionary BencodeToInfoDict(Dictionary<string, object> dict)
		{
			InfoDictionary infoDictionary;
			if (dict.ContainsKey("files"))
			{
				// multiple file info dictionary
				infoDictionary = new MultipleFileMode(dict);
			}
			else
			{
				infoDictionary = new SingleFileMode(dict);
			}
			return infoDictionary;
		}

	}
}