using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'bitfield' is only ever sent as the first message. 
	/// Its payload is a bitfield with each index that downloader has sent set to one 
	/// and the rest set to zero. 
	/// 
  ///	Downloaders which don't have anything yet may skip the 'bitfield' message. 
  ///	The first byte of the bitfield corresponds to indices 0 - 7 from high bit to low bit,
  ///	respectively. The next one 8-15, etc. Spare bits at the end are set to zero.
	/// </summary>
    public class Bitfield : Message, IMessage {

        /* 0 0 0 13 5 255 255 255 255 255 255 255 255 255 255 255 224 */


        #region IMessage Members

        byte[] IMessage.ToStream() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
