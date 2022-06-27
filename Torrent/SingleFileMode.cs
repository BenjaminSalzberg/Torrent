namespace Torrent
{
    public class SingleFileMode : InfoDictionary
    {
        
        // the length of the file in bytes
        public int Length {get; private set;}
        //md5sum a 32-character hexadecimal string corresponding to the MD5 sum of the file. This is not used by BitTorrent at all, but it is included by some programs for greater compatibility.
        public string md5sum {get; private set;}
        public bool md5sumexists {get; private set;} = false;
        public SingleFileMode(Dictionary<String, Object> infoDict)
        {
            Name = System.Text.Encoding.UTF8.GetString((byte[])infoDict["name"]);
            Pieces = piecesSplitter((byte[])infoDict["pieces"]);
            if(Pieces is null)
            {
                Pieces = new byte[0][];
            }
            if(infoDict.ContainsKey("md5sum"))
            {
                md5sum = System.Text.Encoding.UTF8.GetString((byte[])infoDict["md5sum"]);
                if(md5sum is null)
                {
                    md5sum = "";
                }
                md5sumexists = true;
            }
            else
            {
                md5sum = "";
                md5sumexists= false;
            }
        }
    }
}