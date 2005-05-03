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

        public int FileLength {
            get { return fileLength; }
        }
    }

    internal class FileStruct {
        private FileData ioFile;
        private int begin;
        private int length;

        public FileStruct(FileData ioFile, int beginPosition, int length) {
            this.ioFile = ioFile;
            this.begin = beginPosition;
            this.length = length;
        }

        public FileData Io {
            get { return ioFile; }
        }

        public int Begin {
            get { return begin; }
        }

        public int Length {
            get { return length; }
        }
    }

    /// <summary>
	/// A file consist of several Filepieces.
	/// </summary>
	public class File {
        private const int PIECELENGTH = 1 << 18;
        private ArrayList filesData;
        private Hashtable filePieces;
        private int numFiles;
        private int numPieces;
        private Hashtable loadedPieces;
        
        public File(int numPieces) {
            this.numPieces = numPieces;
            filePieces = new Hashtable(numPieces);
            loadedPieces = new Hashtable();
        }

        public File(string fileName, int fileLength, int numPieces) : this(numPieces) {
            FileData data = new FileData(fileName, fileLength);
            filesData = new ArrayList(1);
            filesData.Add(data);
            numFiles = 1;
            CreateFileStruct();
        }

        public File(List fileList, int numPieces) : this(numPieces) {

            numFiles = 0;
            CreateFileStruct();
        }

        #region Private methods

        private void CreateFileStruct() {
            int actualProcesing;
            int pieceIndex = 0;
            int leftOnPiece;
            int actualFileLength;
            FileStruct fileStruct;
            ArrayList filesOnIndex;

            leftOnPiece = PIECELENGTH;
            filesOnIndex = new ArrayList();
            // For each file
            for (int i = 0; i < numFiles; i++) {
                actualProcesing = 0;
                actualFileLength = ((FileData) filesData[i]).FileLength;
                // Process all the file
                while (actualProcesing < actualFileLength) {
                    // The rest can go on this piece
                    if ((actualFileLength - actualProcesing) < leftOnPiece) {
                        leftOnPiece -= (actualFileLength - actualProcesing);
                        fileStruct = new FileStruct(((FileData)filesData[i]), actualProcesing, (actualFileLength - actualProcesing));
                        filesOnIndex.Add(fileStruct);
                        actualProcesing = actualFileLength;
                    }
                    else {
                        fileStruct = new FileStruct(((FileData)filesData[i]), actualProcesing, leftOnPiece - 1);
                        filesOnIndex.Add(fileStruct);
                        actualProcesing += leftOnPiece;
                        filePieces.Add(pieceIndex, filesOnIndex);
                        filesOnIndex = new ArrayList();
                        leftOnPiece = PIECELENGTH;
                        pieceIndex++;
                    }
                }
                // The actual file has been processed.
            }
            // The last file ??
            if (filesOnIndex.Count > 0)
                filePieces.Add(pieceIndex, filesOnIndex);
        }

        private bool PieceOnMemory(int filePieceIndex) {
            return (loadedPieces.ContainsKey(filePieceIndex));
        }

        private void LoadPiece(int filePieceIndex) {
            ArrayList filesOnIndex = (ArrayList)filePieces[filePieceIndex];
            byte[] tempFilePiece = new byte[PIECELENGTH];
            int actualPos = 0;
            for (int i = 0; i < filesOnIndex.Count; i++) {
                // Read from file
                byte[] readed = ((FileStruct)filesOnIndex[i]).Io.so.Read(((FileStruct)filesOnIndex[i]).Begin, ((FileStruct)filesOnIndex[i]).Length);
                for (int j = 0; j < readed.Length; j++)
                    tempFilePiece[actualPos++] = readed[j];
            }
            FilePiece filePiece = new FilePiece(filePieceIndex, PIECELENGTH, tempFilePiece);
            loadedPieces.Add(filePieceIndex, filePiece);
        }

        #endregion

        #region Public methods

        public void WritePiece(Piece piece) {
            // The piece goes to file...
            int pieceOnFile = 0;
            // Write the piece
            ((FileData)filesData[pieceOnFile]).so.Write(piece.PieceIndex, piece.PieceValues);
        }

        public Piece Read(int pieceIndex, int begin, int length) {
            if (pieceIndex >= numPieces)
                throw new FileException("Piece [" + pieceIndex.ToString() + "] out of index.");
            if ((length - begin) > PIECELENGTH)
                throw new FileException("The piece length is not so big.");
            // TODO: Check to see if we already have the piece
            // Check to see if the piece is on memory
            if (!PieceOnMemory(pieceIndex)) {
                // Load Piece
                LoadPiece(pieceIndex);
            }
            FilePiece filePiece = (FilePiece) loadedPieces[pieceIndex];
            Piece requestedPiece = new Piece(pieceIndex, begin, filePiece.Piece(begin, length));
            return requestedPiece;
        }

        public void CloseFiles() {
            for (int i = 0; i < filesData.Count; i++) {
                ((FileData)filesData[i]).so.Close();
            }
        }

        #endregion

    }
}
