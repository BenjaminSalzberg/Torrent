namespace Torrent
{
    public class MultipleFileMode : InfoDictionary
    {
        // a list of dictionaries, one for each file. Each dictionary in this list contains the following keys
        public List<FileDicts> Files {get; private set;}
        public MultipleFileMode(Dictionary<String, Object> infoDict)
        {
            Name = System.Text.Encoding.UTF8.GetString((byte[])infoDict["name"]);
            PieceLength = (long)infoDict["piece length"];
            Files = fileDictList((List<Object>)infoDict["files"]);
            Pieces = piecesSplitter((byte[])infoDict["pieces"]);
            if(Pieces is null)
            {
                Pieces = new byte[0][];
            }
            if(infoDict.ContainsKey("private"))
            {
                if((int)infoDict["private"] == 1)
                {
                    IsPrivate = true;
                }
            }
            else
            {
                IsPrivate = false;
            }
        }

        private List<FileDicts> fileDictList(List<Object> lst)
        {
            List<FileDicts> retLst = new List<FileDicts>();
            foreach(Object FileDict in lst)
            {
                retLst.Add(new FileDicts((Dictionary<String, Object>)FileDict));
            }
            return retLst;
        }
    }
}