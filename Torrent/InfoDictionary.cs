namespace Torrent
{
    public abstract class InfoDictionary
    {
        // number of bytes in each piece
        public long PieceLength {get; protected set;}
        // string consisting of the concatenation of all 20-byte SHA1 hash values, one per piece (byte string, i.e. not urlencoded)
        public byte[][] Pieces {get; protected set;} = new byte[0][];
        // this field is an integer. If it is set to "1", the client MUST publish its presence to get other peers ONLY via the trackers explicitly described in the metainfo file. If this field is set to "0" or is not present, the client may obtain peer from other means, e.g. PEX peer exchange, dht. Here, "private" may be read as "no external peer source".
        public bool IsPrivate {get; protected set;}  
        
        // the files name, or the Directory to store the files in if single or multiple file mode respectively
        public string Name {get; protected set;} = "";

        public InfoDictionary()
        {
            Pieces = new byte[0][];
        }

        protected byte[][] piecesSplitter(byte[] bytearr)
        {
            // given a byte array with a size divisible by 20, convert into a series of byte arrays that is stored in the 2 dimensional byte array
            //Console.WriteLine(bytearr.Length);
            long lengthOfArray = bytearr.Length;
            long NumberofPieces = lengthOfArray/20;
            byte[][] pieces = new byte[NumberofPieces][];
            for (int i = 0; i < NumberofPieces; i++)
            {
                byte[] temparr = new Byte[20];
                for (int q = 0; q < 20; q++)
                {
                    temparr[q] = bytearr[(i*20)+q];
                }
                pieces[i] = temparr.ToArray();
            }
            return pieces;
        }
    } 
}