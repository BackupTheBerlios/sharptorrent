using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'piece' messages contain an index, begin, and piece. 
	/// Note that they are correlated with request messages implicitly. 
	/// It's possible for an unexpected piece to arrive if choke and unchoke messages 
	/// are sent in quick succession and/or transfer is going very slowly.
	/// </summary>
	public class Piece : Message {
		private Int32 index;
		private Int32 begin;
		private byte [] piece;

		public Piece(Int32 index, Int32 begin, byte [] piece) {
			this.messageType = 7;
			this.index = index;
			this.begin = begin;
			this.piece = new byte [piece.Length];
			piece.CopyTo(this.piece, piece.Length);
			DoMessage();
		}

		private void DoMessage() {
			// the message
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1 + 2 * BigEndian.BIGENDIANBYTELENGTH + piece.Length];
      StoreMessageLength(2 * BigEndian.BIGENDIANBYTELENGTH + piece.Length);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
			BigEndian.ToBigEndian(index, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
			BigEndian.ToBigEndian(begin, ref message, 2 * BigEndian.BIGENDIANBYTELENGTH);
		}

		#region Properties

		public int PieceLength {
			get { return piece.Length; }
		}

		#endregion
	}
}
