using System;

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
        private int index;
		private int begin;
		private int length;

		public Cancel(int index, int begin, int length) {
			
			this.index = index;
			this.begin = begin;
    		this.length = length;
		}


        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
