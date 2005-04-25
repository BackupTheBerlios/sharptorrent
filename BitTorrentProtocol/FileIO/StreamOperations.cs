#region Using directives
using System;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Exceptions;
#endregion

namespace SharpTorrent.BitTorrentProtocol.FileIO {

    public class StreamOperations : Stream {
        private const int BUFFERLENGTH = 1024;
        private BinaryWriter bw;
        private BinaryReader br;

        #region Constructors
        
        public StreamOperations(string fileName, int fileLength) {
            this.fileName = fileName;
            try {
                if (!CreateFile(fileLength)) {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
                    br = new BinaryReader(fs);
                    bw = new BinaryWriter(fs);
                }
            }
            catch (FileNotFoundException fnf) {
                throw new StreamOperationsException("File (" + fileName + ") was not found for writing.", fnf);
            }
        }

        #endregion

        #region Private methods

        public bool CreateFile(int fileLength) {
            if (!System.IO.File.Exists(fileName)) {
                // Create file
                fs = new FileStream(fileName, FileMode.CreateNew);
                bw = new BinaryWriter(fs);
                br = new BinaryReader(fs);
                // Write content to 0. Buffer is 1024 bytes.
                byte[] buffer = new byte[BUFFERLENGTH];
                for (int blocks = fileLength / BUFFERLENGTH; blocks > 0; blocks--)
                    bw.Write(buffer);
                byte[] rest = new byte[fileLength % BUFFERLENGTH];
                // Write the ending bytes
                bw.Write(rest);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Public methods

        public void Write(int position, byte[] buffer) {
            try {
                // Position in the file
                bw.Seek(position, SeekOrigin.Begin);
                bw.Write(buffer);
            }
            catch (IOException ioe) {
                throw new StreamOperationsException("Error while writing to position (" + position.ToString() + ") on file (" + fileName + ").", ioe);
            }
        }

        public byte[] Read(int position, int readLength) {
            try {
                byte[] read = new byte[readLength];
                br.Read(read, position, readLength);
                return read;
            }
            catch (IOException ioe) {
                throw new StreamOperationsException("Error while writing to position (" + position.ToString() + ") on file (" + fileName + ").", ioe);
            }
        }

        public void Close() {
            bw.Close();
            br.Close();
            fs.Close();
        }

        #endregion
    }
}
