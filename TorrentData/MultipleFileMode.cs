namespace TorrentController.TorrentData
{
	public class MultipleFileMode : InfoDictionary
	{
		// a list of dictionaries, one for each file. Each dictionary in this list contains the following keys
		public List<FileDicts> Files { get; private set; }
		public MultipleFileMode(Dictionary<string, object> infoDict)
		{
			Name = System.Text.Encoding.UTF8.GetString((byte[])infoDict["name"]);
			PieceLength = (long)infoDict["piece length"];
			Files = FileDictList((List<object>)infoDict["files"]);
			Pieces = PiecesSplitter((byte[])infoDict["pieces"]);
			Pieces ??= Array.Empty<byte[]>();
			if (infoDict.TryGetValue("private", out object? value))
			{
				if ((int)value == 1)
				{
					IsPrivate = true;
				}
			}
			else
			{
				IsPrivate = false;
			}
		}

		private static List<FileDicts> FileDictList(List<object> lst)
		{
			List<FileDicts> retLst = new();
			foreach (object FileDict in lst)
			{
				retLst.Add(new FileDicts((Dictionary<string, object>)FileDict));
			}
			return retLst;
		}
	}
}