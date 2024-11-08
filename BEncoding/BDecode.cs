namespace TorrentController.BEncoding
{
	public static class BDecode
	{

		public static object Decode(string path)
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
			return DecodeRecursion(enumerator);
		}


		private static object DecodeRecursion(IEnumerator<byte> enumerator)
		{
			Bencoding bencoding = (Bencoding)enumerator.Current;
			if (bencoding == Bencoding.DictionaryStart)
			{
				return DecodeDictionary(enumerator);
			}
			else if (bencoding == Bencoding.ListStart)
			{
				return DecodeList(enumerator);
			}
			else if (bencoding == Bencoding.NumberStart)
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
			Dictionary<string, object> dict = new();
			List<string> keys = new();
			while (enumerator.MoveNext())
			{
				if ((Bencoding)enumerator.Current == Bencoding.DictionaryEnd)
				{
					break;
				}
				string key = System.Text.Encoding.UTF8.GetString(DecodeByteArray(enumerator));
				enumerator.MoveNext();
				object val = DecodeRecursion(enumerator);
				keys.Add(key);
				dict.Add(key, val);
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
			List<object> lst = new();
			while (enumerator.MoveNext())
			{
				if ((Bencoding)enumerator.Current == Bencoding.ListEnd)
				{
					break;
				}
				lst.Add(DecodeRecursion(enumerator));
			}
			return lst;
		}
		private static byte[] DecodeByteArray(IEnumerator<byte> enumerator)
		{
			// we are currently looking at the beggining of an integer that dicates the length of the string
			// they are seperated by a divider
			List<byte> lengthBytes = new();
			do
			{
				if ((Bencoding)enumerator.Current == Bencoding.ByteArrayDivider)
					break;
				lengthBytes.Add(enumerator.Current);
			}
			while (enumerator.MoveNext());
			string lengthString = System.Text.Encoding.UTF8.GetString(lengthBytes.ToArray());

			if (!int.TryParse(lengthString, out int length))
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
			List<byte> lst = new();
			// we are now at the first of the values in the number
			while (enumerator.MoveNext())
			{
				if ((Bencoding)enumerator.Current == Bencoding.NumberEnd)
				{
					break;
				}
				lst.Add(enumerator.Current);
			}
			// we now have the entire number as bytes in lst
			// return it as a long
			return long.Parse(System.Text.Encoding.UTF8.GetString(lst.ToArray()));
		}

	}
}