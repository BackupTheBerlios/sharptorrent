using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// KeepAlive message.
	/// </summary>
	public class KeepAlive : Message {
		public KeepAlive() {
			message = new byte [BigEndian.BIGENDIANBYTELENGTH];
		}

		public new string ToString() {
			return "(KeepAlive) ";
		}
	}
}
