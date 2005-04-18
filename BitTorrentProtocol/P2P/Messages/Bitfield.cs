using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'bitfield' (5) is only ever sent as the first message. 
	/// Its payload is a bitfield with each index that downloader has sent set to one 
	/// and the rest set to zero. 
	/// 
  ///	Downloaders which don't have anything yet may skip the 'bitfield' message. 
  ///	The first byte of the bitfield corresponds to indices 0 - 7 from high bit to low bit,
  ///	respectively. The next one 8-15, etc. Spare bits at the end are set to zero.
	/// </summary>
    public class Bitfield : Message, IMessage {
        private const int BITFIELDMESSAGELENGTH = BigEndian.BIGENDIANBYTELENGTH + 1;
        private ArrayList pieces;
        private int numPieces;

        /* 0 0 0 13 5 255 255 255 255 255 255 255 255 255 255 255 224 */

        #region Constructors

        public Bitfield() {
            this.type = 5;
            numPieces = 0;
            pieces = new ArrayList();
        }

        public Bitfield(int numberOfPieces) : base() {
            numPieces = numberOfPieces;
        }

        public Bitfield(Bitfield origin) {

        }

        #endregion

        #region  Public methods

        public void AddPiece(int pieceIndex) {
            if (!pieces.Contains(pieceIndex))
                pieces.Add(pieceIndex);
        }

        #endregion

        #region IMessage Members

        byte[] IMessage.ToStream() {
            // Num bytes to store the bitField (numPieces / 32)
            int numBytesOnMessage = ((int) numPieces / 32) + ((int) numPieces % 32);
            this.message = new byte[BITFIELDMESSAGELENGTH + numBytesOnMessage];
            byte[] messageLength = BigEndian.ToBigEndian(1 + numBytesOnMessage);
            AddMessage(message, messageLength);
            AddMessage(message, type);
            // Convert each piece

            return message;
        }

        #endregion
    }
}
