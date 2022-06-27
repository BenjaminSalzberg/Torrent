using System;
using System.Net;

namespace Torrent
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        public static void Main(string[] args)
        {
            String path = "D:\\Users\\Benjamin\\Downloads\\tears-of-steel.torrent";
            String name = "tears-of-steel.torrent";
            Dictionary<String, Object> obj = (Dictionary<String, Object>)BDecode.Decode(path);
            Torrent torrentDictionary = BencodeToTorrent(obj, name);
            //print(torrentDictionary.TrackerList.Count);
            List<Tracker> trackLst = torrentDictionary.sortedTrackerLst();
            //foreach (var item in trackLst)
            //{
            //    print(item.Name);
            //    print(item.priority);
            //}
            SocketRunner sockets = new SocketRunner();
        }

        public async void sendWebRequest(String url)
        {
            try	
            {
                HttpResponseMessage response = await client.GetAsync("https://" + url.Substring(6));
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
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