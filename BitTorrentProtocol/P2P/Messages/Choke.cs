#region Using directives

using System;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
    public class Choke : Message, IMessage {
        public Choke()
        {

        }

        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
