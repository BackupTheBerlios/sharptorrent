using System;
using SharpTorrent.BitTorrentProtocol.Cryptography;

namespace SharpTorrent.BitTorrentProtocol.FileIO {
	/// <summary>
	/// The peer protocol refers to pieces of the file by index as described in the metainfo
	/// file, starting at zero.
	/// </summary>
	public class FilePiece {
        private int index;
        private int pieceLength;
        private byte[] sha1;
        private byte[] filePiece;

        #region Constructors

        public FilePiece(int index, int pieceLength, byte[] filePiece) {
            this.index = index;
            this.pieceLength = pieceLength;
            this.filePiece = filePiece;
        }

        public FilePiece(int index, int pieceLength, byte [] pieceSHA1, byte [] filePiece) : this(index, pieceLength, filePiece) {
            this.sha1 = pieceSHA1;
        }

        #endregion

        #region Public methods
    
        public byte[] Piece(int begin, int length) {
            return null;
        }

        #endregion
    }
}
