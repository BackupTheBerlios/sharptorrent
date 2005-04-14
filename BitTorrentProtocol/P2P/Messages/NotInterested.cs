#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {

    public class NotInterested : Message, IMessage {

        public NotInterested() {
            this.type = 3;
        }

        #region IMessage Members

        byte[] IMessage.ToStream() {
            base.ToStream();
            message[4] = type;
            return message;
        }

        #endregion
    }
}
