using System;
using System.Security.Cryptography;

namespace SharpTorrent.BitTorrentProtocol.Cryptography {
	/// <summary>
	/// This is SHA1 wrapper
	/// </summary>
	public class SHA1 {
		public const int SHA1SIZE = 20;
		private static System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1Managed();
		//private byte [] sha1Hash;

		public SHA1() {
		}

		/*private byte [] Hash(byte [] buffer) {		
			sha1Hash = sha.ComputeHash(buffer);
			return sha1Hash;
		}*/

		/*public string HashValue(byte [] buffer) {
			return HexEncoding.ToString(Hash(buffer)).ToLower();
		}*/

		public static byte [] HashValue(byte [] buffer) {
			return sha.ComputeHash(buffer);
		}

		
	}
}
