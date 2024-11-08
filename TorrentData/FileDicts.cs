namespace TorrentController.TorrentData
{
	public class FileDicts
	{
		// the length of the file in bytes
		public long Length { get; set; }
		//md5sum a 32-character hexadecimal string corresponding to the MD5 sum of the file. This is not used by BitTorrent at all, but it is included by some programs for greater compatibility.
		public string Md5sum { get; set; }
		public bool Md5sumexists { get; private set; } = false;
		// a list containing one or more string elements that together represent the path and filename. Each element in the list corresponds to either a directory name or (in the case of the final element) the filename. For example, a the file "dir1/dir2/file.ext" would consist of three string elements: "dir1", "dir2", and "file.ext".
		public string path { get; set; }
		public FileDicts(Dictionary<string, object> fileDict)
		{
			Length = (long)fileDict["length"];
			if (fileDict.TryGetValue("md5sum", out object? md5sum))
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
			path = Convertlst((List<object>)fileDict["path"]);
		}
		private static string Convertlst(List<object> pathLst)
		{
			string str = "";
			foreach (object pathPart in pathLst)
			{
				str += System.Text.Encoding.UTF8.GetString((byte[])pathPart);
			}
			return str;
		}
	}
}