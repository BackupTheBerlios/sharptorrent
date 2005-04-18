using System;
using SharpTorrent.BitTorrentProtocol.Exceptions;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// The 'have' message's payload is a single number, 
	/// the index which that downloader just completed and checked the hash of.
	/// "index" must be 4 bytes Big Encoded
	/// </summary>
    public class Have : Message, IMessage {
        /// <summary>
        /// Message Length = 4 bytes big endian.
        /// + 1 byte for message type
        /// + 4 bytes for index.
        /// </summary>
        private const int MESSAGELENGHT = BigEndian.BIGENDIANBYTELENGTH + 1 + 4;
        private int index;

		public Have(int index) {
			this.index = index;
		}

        #region IMessage Members

        byte[] IMessage.ToStream() {
            this.type = 4;
            this.message = new byte [MESSAGELENGHT];
            
            byte[] messageLength = BigEndian.ToBigEndian(MESSAGELENGHT);
            AddMessage(message, messageLength);
            AddMessage(message, type);
            byte[] messageContent = BigEndian.ToBigEndian(index);
            AddMessage(message, messageContent);
            return this.message;
        }

        #endregion
    }
}
