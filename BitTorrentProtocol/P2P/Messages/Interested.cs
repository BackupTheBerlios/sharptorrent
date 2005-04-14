#region Using directives

using System;
using SharpTorrent.BitTorrentProtocol.Utilities;
#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages
{
    public class Interested : Message, IMessage {
        
        public Interested() {
            this.type = 2;
        }

        #region IMessage Members

        byte[] IMessage.ToStream() {
            message = base.ToStream();
            message[4] = type;
            return message;
        }

        #endregion
    }
}
