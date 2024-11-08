namespace TorrentController.BEncoding
{
	public enum Bencoding : byte
	{
		DictionaryStart = 100,
		DictionaryEnd = 101,
		ListStart = 108,
		ListEnd = DictionaryEnd,
		NumberStart = 105,
		NumberEnd = DictionaryEnd,
		ByteArrayDivider = 58
		/*
		static byte DictionaryStart  = System.Text.Encoding.UTF8.GetBytes("d")[0]; // 100
        static byte DictionaryEnd    = System.Text.Encoding.UTF8.GetBytes("e")[0]; // 101
        static byte ListStart        = System.Text.Encoding.UTF8.GetBytes("l")[0]; // 108
        static byte ListEnd          = System.Text.Encoding.UTF8.GetBytes("e")[0]; // 101
        static byte NumberStart      = System.Text.Encoding.UTF8.GetBytes("i")[0]; // 105
        static byte NumberEnd        = System.Text.Encoding.UTF8.GetBytes("e")[0]; // 101
        static byte ByteArrayDivider = System.Text.Encoding.UTF8.GetBytes(":")[0]; //  58
		*/

	}
}