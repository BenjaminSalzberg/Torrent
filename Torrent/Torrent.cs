namespace Torrent
{
    public class Torrent
    {
        // Name of the torrent 
        public string Name {get; private set;}
        // Announce: Name of the tracker (can be a list, can contain just 1 element)
        public List<Tracker> TrackerList {get; private set;} = new List<Tracker>();
        // Creation Date
        public DateTime CreationDate {get; private set;}
        // Comment
        public string Comment = "";
        // Created By
        public string CreatedBy = "";
        // The string encoding format used to generate the pieces part of the info dictionary in the .torrent metafile
        public TorrentEncoding torrentEncoding {get; private set;}
        // the 
        public InfoDictionary infodictionary {get; private set;}

        // Torrent Constructor
        public Torrent(Dictionary<String, Object> dict, String name)
        {
            //name of the torrent file
            this.Name = name;
            // convert to unix timestamp
            if(dict.ContainsKey("creation date"))
                this.CreationDate = DateTimeOffset.FromUnixTimeSeconds((long)dict["creation date"]).DateTime; 
            if(dict.ContainsKey("created by"))
                this.CreatedBy = System.Text.Encoding.UTF8.GetString((byte[])dict["created by"]);
            if(dict.ContainsKey("comment"))
                this.Comment = System.Text.Encoding.UTF8.GetString((byte[])dict["comment"]);

            // tracker list generate using announce and tracker list
            String announce = System.Text.Encoding.UTF8.GetString((byte[])dict["announce"]);
            List<Object> AnnounceLst;
            if(dict.ContainsKey("announce-list"))
            {
                AnnounceLst = (List<Object>)dict["announce-list"];
                this.TrackerList = BencodeToTrackerLst(AnnounceLst);
            }
            else
            {
                this.TrackerList = new List<Tracker>();
                TrackerList.Add(new Tracker(announce));
            }
            
            
            this.torrentEncoding = new TorrentEncoding(System.Text.Encoding.UTF8.GetString((byte[])dict["encoding"]));

            Dictionary<String, Object> infodict = (Dictionary<String, Object>)dict["info"];
            this.infodictionary = BencodeToInfoDict(infodict);
            
            // This is url-list it is not implemented here, it is used to tell you where torrent data may be retrieved. 
            //List<Object> URLLst = (List<Object>)dict["url-list"];
            //Console.WriteLine(URLLst.Count);
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString((byte[])URLLst[0]));
        }
        public List<Tracker> sortedTrackerLst()
        {
            List<Tracker> retLst = new List<Tracker>(TrackerList);
            retLst.Sort(new Comparison<Tracker>((x, y) => x.priority.CompareTo(y.priority)));
            return retLst;
        }
        private static List<Tracker> BencodeToTrackerLst(List<Object> AnnounceLst)
        {
            List<Tracker> trackerlst = new List<Tracker>();
            for (int i = 0; i < AnnounceLst.Count; i++)
            {
                List<Object> sublst = (List<Object>)AnnounceLst[i];
                foreach (var item in sublst)
                {
                    Tracker track = new Tracker(System.Text.Encoding.UTF8.GetString((byte[])item), i);
                    trackerlst.Add(track);
                }
            }
            return trackerlst;
        }
        private static InfoDictionary BencodeToInfoDict(Dictionary<String, Object> dict)
        {
            InfoDictionary infoDictionary;
            if(dict.ContainsKey("files"))
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