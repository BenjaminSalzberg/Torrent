namespace Torrent
{
    public class Tracker
    {
        // Name of the tracker 
        public string Name {get; set;} = "";
        // Announce: Name of the tracker (can be a list, can contain just 1 element)
        // also the url of the tracker

        //used primarily for announce list, prefer lower priority, defaults to 0 for announce
        public int priority {get; set; }


        public Tracker(String name, int priority = 0)
        {
            this.Name = name;
            this.priority = priority;
        }
    } 
}