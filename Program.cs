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
            TrackerCommunication communication = new TrackerCommunication();
            foreach (Tracker tracker in torrentDictionary.TrackerList)
            {
                string announce = tracker.Name;
                Console.WriteLine("Starting tracker " + announce);
                try
                {
                    communication.SendTrackerCommunication(tracker, torrentDictionary.info_hash);
                }
                catch(Exception e) when (e.Message == "This protocol has not been implemented")
                {
                    Console.WriteLine(e);
                    continue;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                if(tracker.trackerResponse != null)
                {
                    //Console.WriteLine(BitConverter.ToString(tracker.trackerResponse));
                    break;
                }
            }
            var activeTrackers = torrentDictionary.TrackerList.Where(x=> x.status == TrackerStatus.Active);
            foreach (var item in activeTrackers)
            {
                Console.WriteLine(item.Name);
                if(item.transaction_id != null)
                    Console.WriteLine(BitConverter.ToString(item.transaction_id));
                if(item.connection_id != null)
                    Console.WriteLine(BitConverter.ToString(item.connection_id));
            }
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