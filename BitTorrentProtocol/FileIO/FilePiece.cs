using System;
using SharpTorrent.BitTorrentProtocol.Cryptography;

namespace SharpTorrent.BitTorrentProtocol.FileIO {
	/// <summary>
	/// The peer protocol refers to pieces of the file by index as described in the metainfo
	/// file, starting at zero.
	/// </summary>
	public class FilePiece {
        private int index;
        private int pieceLebgth;
        private byte[] sha1;

        public FilePiece(int index, int pieceLength, byte [] pieceSHA1) {
		}
	}
}
