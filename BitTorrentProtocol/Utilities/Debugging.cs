using System;
using System.Diagnostics;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;

namespace SharpTorrent.BitTorrentProtocol.Utilities {
	/// <summary>
	/// Summary description for Debugging.
	/// </summary>
	public class Debugging {
		//private int level = 999;

		public Debugging() {
		}

		public static void BufferToDebug(byte [] buffer) {
			Debug.WriteLine("Received: " + HexEncoding.ToString(buffer));
			Debug.Write("Received byte values: ");
			foreach(byte bit in buffer)
				Debug.Write(bit.ToString());
			Debug.WriteLine("");
		}

		public static void StringToDebug(string message) {
			StringToDebug(message, null);
		}

		public static void StringToDebug(string message, string category) {
			if (category != null)
				Debug.WriteLine(message, category);
			else
				Debug.WriteLine(message);
		}

		public static void MessageToDebug(Message message) {
			Debug.WriteLine(message, "Messages");
		}
	}
}
