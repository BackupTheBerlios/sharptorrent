using System;
using SharpTorrent.BitTorrentProtocol.Types;

namespace SharpTorrent.BitTorrentProtocol
{
	/// <summary>
	/// The tracker has an info dictionary.
	/// 
	/// More info:
	/// 
	/// For the purposes of transfer, files are split into fixed-size pieces which are all 
	/// the same length except for possibly the last one which may be truncated. 
	/// Piece length is almost always a power of two, most commonly 218 = 256 K 
	/// (BitTorrent prior to version 3.2 uses 220 = 1 M as default).
	/// 
	/// There is also a key <b>length</b> or a key <b>files</b>, but <u>not both or neither.</u>
	/// If length is present then the download represents a single file, 
	/// otherwise it represents a set of files which go in a directory structure.
	/// 
	/// </summary>
	public class InfoDictionary : Types.Dictionary {
		/// <summary>
		/// The <b>name</b> key maps to a string which is the suggested name to save 
		/// the file (or directory) as. It is purely advisory.
		/// </summary>
		private string name;
		/// <summary>
		/// <b>piece length</b> maps to the number of bytes in each piece the file is split into. 
		/// </summary>
		private int pieceLength;
		/// <summary>
		/// <b>pieces</b> maps to a string whose length is a multiple of 20. It is to be 
		/// subdivided into strings of length 20, each of which is the SHA1 hash of the 
		/// piece at the corresponding index.
		/// </summary>
		private Types.String pieces;
		/// <summary>
		/// This indicates if the case is a single,<b>true</b>, or multi-file, <b>false</b>.
		/// </summary>
		private bool isSingleFile;
		/// <summary>
		/// <b>length</b> maps to the length of the file in bytes.
		/// </summary>
		private int length;
		/// <summary>
		/// The multi-file case is treated as only having a single file by concatenating the 
		/// files in the order they appear in the files list. 
		/// The files list is the value <b>files</b> maps to, and is a <i>list of dictionaries</i> containing
		/// the following keys:
		/// <b>length</b> The length of the file, in bytes. 
		///	<b>path</b> A list of strings corresponding to subdirectory names, the last of which
		///	is the actual file name (a zero length list is an error case).
		///	In the single file case, the name key is the name of a file.
		///	In the muliple file case, it's the name of a directory.
		/// </summary>
		private Dictionary files;
		//private byte [] buffer;

		public InfoDictionary() {
		}
		public InfoDictionary(Dictionary info) {
			name = ((Types.String) info["name"]).ToString();
			pieceLength = ((Types.Integer) info["piece length"]).ToInt;
			pieces = (Types.String) info["pieces"];
			// Is it a single file ?
			isSingleFile = (info.ContainsKey("length"));
			if (isSingleFile) {
				length = ((Types.Integer) info["length"]).ToInt;
			}
			else
				length = 0;
			//buffer = (byte []) info["toSHA1info"];
		}

		#region Propiedades

		public string Name {
			get { return name; }
		}

		public int PieceLength {
			get { return pieceLength; }
		}

		public Types.String Pieces {
			get { return pieces; }
		}

		public int Length {
			get { return length; }
		}

		public bool IsASingleFile {
			get { return isSingleFile; }
		}

		/*public byte [] Buffer {
			get { return buffer; }
		}*/

		#endregion
	}
}
