using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'piece' messages contain an index, begin, and piece. 
	/// Note that they are correlated with request messages implicitly. 
	/// It's possible for an unexpected piece to arrive if choke and unchoke messages 
	/// are sent in quick succession and/or transfer is going very slowly.
	/// </summary>
    public class Piece : Message, IMessage {
        private const int MESSAGELENGTH = BigEndian.BIGENDIANBYTELENGTH + 1 + (2 * BigEndian.BIGENDIANBYTELENGTH);
        private int index;
		private int begin;
		private byte [] piece;

		public Piece(int index, int begin, byte [] piece) {
			this.type = 7;
			this.index = index;
			this.begin = begin;
            this.piece = piece;
		}

     
        #region IMessage Members

        byte[] IMessage.ToStream() {
            message = new byte[MESSAGELENGTH + piece.Length];
            byte[] messageLength = BigEndian.ToBigEndian(1 + (2 * BigEndian.BIGENDIANBYTELENGTH) + piece.Length);
            AddMessage(message, messageLength);
            byte[] bIndex = BigEndian.ToBigEndian(index);
            AddMessage(message, bIndex);
            byte[] bBegin = BigEndian.ToBigEndian(begin);
            AddMessage(message, bBegin);
            AddMessage(message, piece);
            return message;
        }

        #endregion

        #region Properties

        public int PieceIndex {
            get { return index; }
        }

        public int PieceOffset {
            get { return begin; }

        }

        public byte[] PieceValues {
            get { return piece; }
        }

        #endregion
    }
}
