using System;
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
    public class Cancel : Message, IMessage {
        private const int MESSAGELENGHT = BigEndian.BIGENDIANBYTELENGTH + 1 + (3 * BigEndian.BIGENDIANBYTELENGTH);
        private int index;
		private int begin;
		private int length;

		public Cancel(int index, int begin, int length) {
            this.type = 8;
            this.index = index;
			this.begin = begin;
    		this.length = length;
		}


        #region IMessage Members

        byte[] IMessage.ToStream() {
            message = new byte[MESSAGELENGHT];
            byte[] messageLength = BigEndian.ToBigEndian(1 + (3 * BigEndian.BIGENDIANBYTELENGTH));
            AddMessage(message, messageLength);
            AddMessage(message, type);
            byte[] bIndex = BigEndian.ToBigEndian(index);
            AddMessage(message, bIndex);
            byte[] bBegin = BigEndian.ToBigEndian(begin);
            AddMessage(message, bBegin);
            byte[] bLength = BigEndian.ToBigEndian(length);
            AddMessage(message, bLength);
            return message;
        }

        #endregion
    }
}
