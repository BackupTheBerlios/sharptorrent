#region Using directives

using System;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
    public class Choke : Message, IMessage {
        
        public Choke() {
            this.type = 0;
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
