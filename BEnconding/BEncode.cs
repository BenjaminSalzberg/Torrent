namespace Torrent
{
    public class BEncode
    {
        
        public static void Encode(object obj, string path)
        // given a torrent file
        // convert it into a byte array 
        {
            MemoryStream buffer = new MemoryStream();
            EncodeRecursion(buffer, obj);
            // write file
            byte[] bytearr = buffer.ToArray();
            buffer.Flush();
            
        }

        private static void EncodeRecursion(MemoryStream buffer, object obj)
        {
            if (obj is byte[])                
                EncodeByteArray(buffer, (byte[])obj);
            else if (obj is string)
                EncodeString(buffer, (string)obj);
            else if (obj is long)
                EncodeNumber(buffer, (long)obj);
            else if (obj.GetType() == typeof(List<object>))
                EncodeList(buffer, (List<object>)obj);
            else if (obj.GetType() == typeof(Dictionary<string,object>))
                EncodeDictionary(buffer, (Dictionary<string,object>)obj);
            else
                throw new Exception("unable to encode type " + obj.GetType());
        }


        private static void EncodeNumber(MemoryStream buffer, long input)
        {
            buffer.Append(Bencoding.NumberStart);
            buffer.Append(System.Text.Encoding.UTF8.GetBytes(Convert.ToString(input)));
            buffer.Append(Bencoding.NumberEnd);
        }
        
        private static void EncodeByteArray(MemoryStream buffer, byte[] bytearr)
        {
            buffer.Append(System.Text.Encoding.UTF8.GetBytes(Convert.ToString(bytearr.Length)));
            buffer.Append(Bencoding.ByteArrayDivider);
            buffer.Append(bytearr);
        }
        private static void EncodeString(MemoryStream buffer, String str)
        {
            EncodeByteArray(buffer, System.Text.Encoding.UTF8.GetBytes(str));
        }
        private static void EncodeList(MemoryStream buffer, List<object> input)
        {
            buffer.Append(Bencoding.ListStart);
            foreach(var item in input)
            {
                EncodeRecursion(buffer, item);
            }
            buffer.Append(Bencoding.ListEnd);
        }
        private static void EncodeDictionary(MemoryStream buffer, Dictionary<string,object> input)
        {
            buffer.Append(Bencoding.DictionaryStart);
            var sortedKeys = input.Keys.ToList().OrderBy(x => BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(x)));
            foreach(var key in sortedKeys)
            {
                EncodeString(buffer, key);
                EncodeRecursion(buffer, input[key]);
            }
            buffer.Append(Bencoding.DictionaryEnd);
        }
    }
    public static class MemoryStreamExtensions
    {
        public static void Append(this MemoryStream stream, byte value)
        {
            stream.Append(new[] { value });
        }

        public static void Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }
    }

}