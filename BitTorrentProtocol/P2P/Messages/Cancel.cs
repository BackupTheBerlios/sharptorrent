using System;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'cancel' messages have the same payload as request messages. 
	/// They are generally only sent towards the end of a download, during what's 
	/// called 'endgame mode'. 
	/// When a download is almost complete, there's a tendency for the last few pieces to 
	/// all be downloaded off a single hosed modem line, taking a very long time. 
	/// To make sure the last few pieces come in quickly, once requests for all pieces 
	/// a given downloader doesn't have yet are currently pending, 
	/// it sends requests for everything to everyone it's downloading from. 
	/// To keep this from becoming horribly inefficient, it sends cancels to everyone 
	/// else every time a piece arrives.
	/// </summary>
	public class Cancel : Message {
		private Int32 index;
		private Int32 begin;
		private Int32 length;

		public Cancel(Int32 index, Int32 begin, Int32 length) {
			this.messageType = 6;
			this.index = index;
			this.begin = begin;
			this.length = length;
			DoMessage();
		}

		private void DoMessage() {
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1 + BigEndian.BIGENDIANBYTELENGTH * 3];
			StoreMessageLength(3 * BigEndian.BIGENDIANBYTELENGTH);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
			BigEndian.ToBigEndian(index, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
			BigEndian.ToBigEndian(begin, ref message, 2 * BigEndian.BIGENDIANBYTELENGTH);
			BigEndian.ToBigEndian(length, ref message, 3 * BigEndian.BIGENDIANBYTELENGTH);
		}
	}
}
