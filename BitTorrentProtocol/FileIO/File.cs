#region Using directives
using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.FileIO;
using SharpTorrent.BitTorrentProtocol.Exceptions;
using SharpTorrent.BitTorrentProtocol.BeEncode;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;
#endregion

namespace SharpTorrent.BitTorrentProtocol.FileIO  {
    internal class FileData {
        private string fileName;
        private int fileLength;
        public StreamOperations so;

        public FileData(string fileName, int fileLength) {
            this.fileName = fileName;
            this.fileLength = fileLength;
            // Create the files
            so = new StreamOperations(fileName, fileLength);
        }
    }
    /// <summary>
	/// A file consist of several Filepieces.
	/// </summary>
	public class File {
        private ArrayList files;

        public File(string fileName, int fileLength) {
            FileData data = new FileData(fileName, fileLength);
            files = new ArrayList(1);
            files.Add(data);
        }

        public File(List fileList) {

        }

        #region Private methods

   
        #endregion

        #region Public methods

        public void WritePiece(Piece piece) {
            // The piece goes to file...
            int pieceOnFile = 0;
            // Write the piece
            ((FileData)files[pieceOnFile]).so.Write(piece.PieceIndex, piece.PieceValues);
        }

        public Piece Read(int pieceIndex) {
            return null;
        }

        public void CloseFiles() {
            for (int i = 0; i < files.Count; i++) {
                ((FileData)files[i]).so.Close();
            }
        }

        #endregion

    }
}
