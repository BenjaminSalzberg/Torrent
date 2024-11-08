namespace TorrentController.TorrentData
{
	public class SingleFileMode : InfoDictionary
	{

		// the length of the file in bytes
		public int Length { get; private set; }
		//md5sum a 32-character hexadecimal string corresponding to the MD5 sum of the file. This is not used by BitTorrent at all, but it is included by some programs for greater compatibility.
		public string Md5sum { get; private set; }
		public bool Md5sumexists { get; private set; } = false;
		public SingleFileMode(Dictionary<string, object> infoDict)
		{
			Name = System.Text.Encoding.UTF8.GetString((byte[])infoDict["name"]);
			Pieces = PiecesSplitter((byte[])infoDict["pieces"]);
			Pieces ??= Array.Empty<byte[]>();
			if (infoDict.TryGetValue("md5sum", out object? md5sum))
			{
				Md5sum = System.Text.Encoding.UTF8.GetString((byte[])md5sum);
				Md5sum ??= "";
				Md5sumexists = true;
			}
			else
			{
				Md5sum = "";
				Md5sumexists = false;
			}
		}
	}
}