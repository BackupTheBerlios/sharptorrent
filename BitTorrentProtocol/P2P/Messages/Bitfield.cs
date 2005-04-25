using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Exceptions;
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
	/// Example:
    /// 0 0 0 13 5 255 255 255 255 255 255 255 255 255 255 255 224
    /// </summary>
    public class Bitfield : Message, IMessage {
        private const int MESSAGELENGTH = BigEndian.BIGENDIANBYTELENGTH + 1;
        private const int BITS = 8;
        private int numPieces;
        private int numBytes;
        private byte[] pieces;


        #region Constructors

        public Bitfield(int numPieces) {
            this.numPieces = numPieces;
            // Create the byte array. 8 pieces = 1 Byte
            numBytes = (numPieces / BITS);
            if ((numPieces % BITS) != 0)
                numBytes++;
            pieces = new byte[numBytes];

        }

        public Bitfield(int numPieces, byte [] buffer) {
            this.numPieces = numPieces;
            pieces = new byte[numPieces];
            for (int i = 0; i < buffer.Length; i++)
                pieces[i] = buffer[i];
        }

        public Bitfield(Bitfield bitfield) {
            numPieces = bitfield.numPieces;
            pieces = new byte[numPieces];
            for (int i = 0; i < bitfield.pieces.Length; i++)
                pieces[i] = bitfield.pieces[i];
        }

        #endregion

        #region Public Methods

        public void AddPiece(int pieceIndex) {
            int byteIndex = (pieceIndex / BITS);
            byte offset = (byte) (pieceIndex % BITS);
            byte maskByte;
            if (offset == 0) {
                byteIndex--;
                maskByte = 128;
            }
            else
                maskByte = (byte) (1 << (offset - 1));
            if (byteIndex >= pieces.Length)
                throw new MessageException("Error adding piece (" + pieceIndex.ToString() + ") to Bitfield.");
            else
                pieces[byteIndex] |= maskByte;
        }

        public bool HasPiece(int pieceIndex) {
            int byteIndex = (pieceIndex / BITS);
            byte offset = (byte)(pieceIndex % BITS);
            byte maskByte;
            if (offset == 0) {
                byteIndex--;
                maskByte = 128;
            }
            else
                maskByte = (byte)(1 << (offset - 1));
            if (byteIndex >= pieces.Length)
                throw new MessageException("There is no piece (" + pieceIndex.ToString() + ").");
            else
                return ((pieces[byteIndex] &= maskByte) > 0);
        }

        #endregion

        #region IMessage Members

        byte[] IMessage.ToStream() {
            message = new byte[MESSAGELENGTH + pieces.Length];
            AddMessage(message, BigEndian.ToBigEndian(message.Length));
            AddMessage(message, type);
            // Add pieces
            AddMessage(message, pieces);
            return message;
        }

        #endregion
    }
}
