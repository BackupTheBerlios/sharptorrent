#region Using directives
using System;
using System.IO;
using SharpTorrent.BitTorrentProtocol.BeEncode;
#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P {
    /// <summary>
    /// This class represents a metaInfo file.
    /// Metainfo files are bencoded dictionaries with the following keys:
    ///
    /// announce -  The URL of the tracker.
    /// info - This maps to a dictionary, with keys described below.
    /// The <b>name</b> key maps to a string which is the suggested name to save the file (or directory) as. It is purely advisory.
    /// <b>piece length</b> maps to the number of bytes in each piece the file is split into. For the purposes of transfer, files are split into fixed-size pieces which are all the same length except for possibly the last one which may be truncated. Piece length is almost always a power of two, most commonly 218 = 256 K (BitTorrent prior to version 3.2 uses 220 = 1 M as default).
    /// <b>pieces</b> maps to a string whose length is a multiple of 20. It is to be subdivided into strings of length 20, each of which is the SHA1 hash of the piece at the corresponding index.
    /// There is also a key <b>length</b> or a key <b>files</b>, but not both or neither. If length is present then the download represents a single file, otherwise it represents a set of files which go in a directory structure.
    /// In the single file case, length maps to the length of the file in bytes.
    /// For the purposes of the other keys, the multi-file case is treated as only having a single file by concatenating the files in the order they appear in the files list. The files list is the value files maps to, and is a list of dictionaries containing the following keys:
    /// <b>length</b> - The length of the file, in bytes. 
    /// <b>path</b> - A list of strings corresponding to subdirectory names, the last of which is the actual file name (a zero length list is an error case). 
    /// In the single file case, the name key is the name of a file, in the muliple file case, it's the name of a directory.
    /// </summary>
    public class MetainfoFileException : Exception {
		public MetainfoFileException() : base() {
		}
		public MetainfoFileException(string message) : base(message) {
		}
        public MetainfoFileException(string message, Exception innerException) : base(message, innerException) {
        }
    }
    public class MetainfoFile {
        private string fileName;
        private BeEncode.String announce = null;
        private BeEncode.Dictionary info = null;

        public MetainfoFile(string fileName) {
            this.fileName = fileName;
            // Check file exists
            if (!File.Exists(this.fileName))
                throw new MetainfoFileException("Can't find (" + this.fileName + ").");
        }

        private void ParseFile() {
            BeEncode.Dictionary metainfo;
            BeParser parser = new BeParser();
            // Load file
            try {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                byte[] buffer = new byte[fs.Length];
                int readBytes = fs.Read(buffer, 0, buffer.Length);
                if (readBytes == 0)
                    throw new MetainfoFileException("There is no metaInfo information on file (" + fileName + ")");
                // We have the buffer full of data, Parse it !!!!
                metainfo = parser.Parse(buffer);
                announce = (BeEncode.String)metainfo["announce"];
                info = (BeEncode.Dictionary)metainfo["info"];
            }
            catch (NotSupportedException nse) {
                throw new MetainfoFileException("Can't read from metaInfo file. [" + nse.Message + "]");
            }
            catch (IOException ioe) {
                throw new MetainfoFileException("Input/Output error. [" + ioe.Message + "]");
            }
            catch (ObjectDisposedException ode) {
                throw new MetainfoFileException("File already closed. [" + ode.Message + "]");
            }
        }

        public void LoadMetainfoFile() {
            try {
                ParseFile();
            }
            catch (BeParserException bp){
                throw new MetainfoFileException("Invalid Metainfo file (" + fileName + ") format. [" + bp.Message + "]");
            }

        }

        public object InfoKey(string dictionaryKey) {
            return info[dictionaryKey];
        }

        #region Properties

        public string Announce {
            get { return announce.StringValue; }
        }

        public BeEncode.Dictionary Info {
            get { return info; }
        }

        #endregion
    }
}
