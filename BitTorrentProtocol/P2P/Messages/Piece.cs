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
        private int index;
		private int begin;
		//private byte [] piece;

		public Piece(int index, int begin, byte [] piece) {
			this.type = 7;
			this.index = index;
			this.begin = begin;
		}

        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
