#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
    public class UnChoke : Message, IMessage {
        public UnChoke()
        {

        }

        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
