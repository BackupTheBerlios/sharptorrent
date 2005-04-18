using System;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'request' messages contain an index, begin, and length. 
	/// The last two are byte offsets. 
	/// Length is generally a power of two unless it gets truncated by the end of the file. 
	/// All current implementations use 2_15, and close connections which request an 
	/// amount greater than 2_17.
	/// </summary>
	public class Request : Message, IMessage {
        private const int MESSAGELENGHT = BigEndian.BIGENDIANBYTELENGTH + 1 + (3 * BigEndian.BIGENDIANBYTELENGTH);
        private int index;
		private int begin;
		private int length;

		public Request(int index, int begin, int length) {
			this.type = 6;
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
