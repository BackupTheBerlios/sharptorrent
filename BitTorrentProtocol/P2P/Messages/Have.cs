using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// The 'have' message's payload is a single number, 
	/// the index which that downloader just completed and checked the hash of.
	/// </summary>
	public class Have :IMessage, Message {
		private Int32 index;

		public Have(Int32 index) {
			this.messageType = 4;
			this.index = index;
			DoMessage();
		}

		private void DoMessage() {
			// the message
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1 + BigEndian.BIGENDIANBYTELENGTH];
			StoreMessageLength(1);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
			BigEndian.ToBigEndian(index, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
		}
	}
}
