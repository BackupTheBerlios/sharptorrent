using System;

namespace SharpTorrent.BitTorrentProtocol.Utilities {
	/// <summary>
	/// We can know if a byte represents a digit
	/// </summary>
	public class Comparations {
		public Comparations() {
		}

		public static bool IsNumeric(byte theByte) {
			return ( (theByte >= 48) && (theByte <= 57));
		}
	}
}
