using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.FileIO;

namespace SharpTorrent.BitTorrentProtocol.FileIO  {
	public delegate void PieceCompleted(Int32 index);
	/// <summary>
	/// A file consist of several pieces.
	/// </summary>
	public class File {
		private string fileName;
		private string filePath;
		private Types.String piecesSHA1;
		private bool isSingleFile;
		private bool completed = false;
		private Int32 length;
		private Int32 pieceLength;
		private Int32 piecesCount;
		private Int32 lastPieceLength;
		private Types.Dictionary fileNames;
		private Piece [] pieces;
		// Events
		public event PieceCompleted OnPieceCompleted;
		
		public File(MetaInfo metaInfoFile) : this(metaInfoFile, "C:\\TEMP\\") {			
		}
	
		public File(MetaInfo metaInfoFile, string storePath) {
			fileName = metaInfoFile.Info.Name;
			filePath = storePath;
			piecesSHA1 = metaInfoFile.Info.Pieces;
			isSingleFile = metaInfoFile.Info.IsASingleFile;
			length = metaInfoFile.Info.Length;
			pieceLength = metaInfoFile.Info.PieceLength;
			piecesCount = length / pieceLength;
			lastPieceLength = length - (piecesCount * pieceLength);
			/*if (!isSingleFile) 
				fileNames = */
			pieces = new Piece [piecesCount];
		}

		/// <summary>
		/// Stores actual pieces data o the file
		/// </summary>
		private void Flush() {
		}

		/// <summary>
		/// Creates the file/s as the metaInfoFile dictates.
		/// </summary>
		private void CreateFiles() {
		}
		
		/// <summary>
		/// If file/s exists then check what we have and what we don't.
		/// </summary>
		private void CheckFiles() {
		}

		public void Write(Int32 index, Int32 beginPos, byte [] data) {

		}

		public void Read(Int32 index, Int32 beginPos, Int32 length, ref byte [] data) {

		}
	}
}
