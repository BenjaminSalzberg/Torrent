namespace Torrent
{
    public static class BDecode
    {
        
        public static Object Decode(string path)
        {
            byte[] torrentFile;
            try
            {
                 torrentFile = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                throw new Exception("Exception Occured: \n" + e.ToString());
            }
            IEnumerator<byte> enumerator = ((IEnumerable<byte>)torrentFile).GetEnumerator();
            enumerator.MoveNext();
            return decodeRecursion(enumerator);
        }


        private static object decodeRecursion(IEnumerator<byte> enumerator)
        {
            if(enumerator.Current == Bencoding.DictionaryStart)
            {
                return DecodeDictionary(enumerator);
            }
            else if (enumerator.Current == Bencoding.ListStart)
            {
                return DecodeList(enumerator);
            }
            else if(enumerator.Current == Bencoding.NumberStart)
            {
                return DecodeNumber(enumerator);
            }
            else
            {
                return DecodeByteArray(enumerator);
            }
        }
        private static Dictionary<string, object> DecodeDictionary(IEnumerator<byte> enumerator)
        {
            Dictionary<string,object> dict = new Dictionary<string,object>();
            List<string> keys = new List<string>();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current==Bencoding.DictionaryEnd)
                {
                    break;
                }
                string key = System.Text.Encoding.UTF8.GetString(DecodeByteArray(enumerator));
                enumerator.MoveNext();
                object val = decodeRecursion(enumerator);
                keys.Add(key);
                dict.Add(key,val);
            }
            var sortedKeys = keys.OrderBy(x => BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(x)));
            if (!keys.SequenceEqual(sortedKeys))
            {
                throw new Exception("error loading dictionary: keys not sorted");
            }
            return dict;
        }
        
        private static List<object> DecodeList(IEnumerator<byte> enumerator)
        {
            List<object> lst = new List<object>();
            while(enumerator.MoveNext())
            {
                if(enumerator.Current==Bencoding.ListEnd)
                {
                    break;
                }
                lst.Add(decodeRecursion(enumerator));
            }
            return lst;
        }
        private static byte[] DecodeByteArray(IEnumerator<byte> enumerator)
        {
            // we are currently looking at the beggining of an integer that dicates the length of the string
            // they are seperated by a divider
            List<byte> lengthBytes = new List<byte>();
            do
            {
                if(enumerator.Current == Bencoding.ByteArrayDivider)
                    break;
                lengthBytes.Add(enumerator.Current);
            }
            while (enumerator.MoveNext());
            string lengthString = System.Text.Encoding.UTF8.GetString(lengthBytes.ToArray());

            int length;
            if (!Int32.TryParse(lengthString, out length))
                throw new Exception("unable to parse length of byte array");
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                enumerator.MoveNext();
                bytes[i] = enumerator.Current;
            }
            return bytes;
        }
        private static long DecodeNumber(IEnumerator<byte> enumerator)
        {

            // we are currently pointing to the starting value for the long. Therefore we can move to the next. 
            List<byte> lst = new List<byte>();
            // we are now at the first of the values in the number
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == Bencoding.NumberEnd)
                {
                    break;
                }
                lst.Add(enumerator.Current);
            }
            // we now have the entire number as bytes in lst
            // return it as a long
            return Int64.Parse(System.Text.Encoding.UTF8.GetString(lst.ToArray()));
        }

    }
}