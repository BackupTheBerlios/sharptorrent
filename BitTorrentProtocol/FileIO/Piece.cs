using System;
using SharpTorrent.BitTorrentProtocol.Cryptography;

namespace SharpTorrent.BitTorrentProtocol.FileIO {
	/// <summary>
	/// The peer protocol refers to pieces of the file by index as described in the metainfo
	/// file, starting at zero.
	/// </summary>
	public class Piece {
		private Int32 index;
		private bool stored = false;
		private byte [] pieceBuffer;
		private byte [] pieceSHA1;
		private Int32 bitsCount = 0;
		// Events
		public event PieceCompleted OnPieceCompleted;

		public Piece(Int32 index, Int32 pieceLength, byte [] pieceSHA1) {
			this.index = index;
			pieceBuffer = new byte [pieceLength];
			this.pieceSHA1 = pieceSHA1;
		}

		private void CheckSHA1() {
			// Calculate SHA1 of the buffer
			byte [] bufferSha1 = SHA1.HashValue(pieceBuffer);
			// Compare to pieceSHA1
			bool equals = true;
			for (int i = 0; i < bufferSha1.Length; i++) {
				if (bufferSha1[i] != pieceSHA1[i]) {
					equals = false;
					break;
				}
			}
			if (equals) {
				if (OnPieceCompleted != null)
					OnPieceCompleted(this.index);			
			}
		}

		public void Write(Int32 initPosition, byte [] data) {
			// How much have we written
			bitsCount += data.Length;
		}

		public void Read(Int32 initPosition, Int32 endPosition, ref byte [] data) {
		}

		public void Flush(/* filehandle */) {
			stored = true;
		}

		#region Properties

		public Int32 PieceIndex {
			get { return index; }
		}

		public bool PieceStored {
			get { return stored; }
		}

		#endregion
	}
}
