using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// The 'have' message's payload is a single number, 
	/// the index which that downloader just completed and checked the hash of.
	/// </summary>
    public class Have : Message, IMessage {
        private int index;

		public Have(int index) {

			this.index = index;
		}

        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
