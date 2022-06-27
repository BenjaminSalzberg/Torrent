namespace Torrent
{
    public class FileDicts
    {
        // the length of the file in bytes
        public long Length {get; set;}
        //md5sum a 32-character hexadecimal string corresponding to the MD5 sum of the file. This is not used by BitTorrent at all, but it is included by some programs for greater compatibility.
        public string md5sum {get; set;}
        public bool md5sumexists {get; private set;} = false;
        // a list containing one or more string elements that together represent the path and filename. Each element in the list corresponds to either a directory name or (in the case of the final element) the filename. For example, a the file "dir1/dir2/file.ext" would consist of three string elements: "dir1", "dir2", and "file.ext".
        public string path {get; set;}
        public FileDicts(Dictionary<String, Object> fileDict)
        {
            Length = (long)fileDict["length"];
            if(fileDict.ContainsKey("md5sum"))
            {
                md5sum = System.Text.Encoding.UTF8.GetString((byte[])fileDict["md5sum"]);
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
            path = convertlst((List<Object>)fileDict["path"]);
        }
        private String convertlst(List<Object> pathLst)
        {
            String str = "";
            foreach (Object pathPart in pathLst)
            {
                str+=(System.Text.Encoding.UTF8.GetString((byte[])pathPart));
            }
            return str;
        }
    }
}