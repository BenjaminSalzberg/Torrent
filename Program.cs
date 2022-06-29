using System;
using System.Net;

namespace Torrent
{
    class Program
    {
        public static readonly HttpClient client = new HttpClient();
        public static void Main(string[] args)
        {
            String path = "D:\\Users\\Benjamin\\Downloads\\tears-of-steel.torrent";
            String name = "tears-of-steel.torrent";
            Dictionary<String, Object> obj = (Dictionary<String, Object>)BDecode.Decode(path);
            Torrent torrentDictionary = BencodeToTorrent(obj, name);
            String announce = System.Text.Encoding.UTF8.GetString((byte[])obj["announce"]);
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
            TrackerCommunication communication = new TrackerCommunication();
            communication.SendTrackerCommunication(announce, torrentDictionary.info_hash);
            //communication.sendWebRequest(announce);
        }

        private static Torrent BencodeToTorrent(Dictionary<String, Object> dict, String name)
        {
            Torrent torrent = new Torrent(dict, name);
            return torrent;
        }
        public static void print(object str)
        {
            Console.WriteLine(str);
        }
    }
}