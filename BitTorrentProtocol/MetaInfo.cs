using System;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.Utilities;
using SharpTorrent.BitTorrentProtocol.Cryptography;

namespace SharpTorrent.BitTorrentProtocol {
	/// <summary>
	/// MetaInfo representation.
	/// 
	/// Metainfo files are <b>bencoded dictionaries</b> with the following keys:
	/// 
	/// announce
	/// info
	/// </summary>
	public class MetaInfo {
		private Dictionary metaInfo;
		private string announce;
		private InfoDictionary info;
		private BeParser parser;
		/// <summary>
		/// This is the SHA1 hash of the Info Dictionary
		/// </summary>
		private byte [] infoHash;

		public MetaInfo() {
		}

		public MetaInfo(string fileName) {
			Load(fileName);
		}

		public void Store(string fileName) {

		}

		/// <summary>
		/// This method read a file an creates a MetaInfo object.
		/// </summary>
		/// <param name="fileName"></param>
		public void Load(string fileName) {
			// Read the file
			FileStream fs = new FileStream(fileName, FileMode.Open);
			Byte [] textFile = new Byte[fs.Length];
			fs.Read(textFile, 0, textFile.Length);
			fs.Close();
			parser = new BeParser(textFile);
			// It's a dictionary, so Deencoded !
			metaInfo = Dictionary.Decode(parser);
			// Get fields
			try {
				announce = ((Types.String) metaInfo["announce"]).ToString();
				Dictionary dInfo = (Dictionary) metaInfo["info"];
				info = new InfoDictionary(dInfo);
				//SHA1 sha = new SHA1();
				infoHash = SHA1.HashValue((byte []) metaInfo["toSHA1info"]);
				Debugging.StringToDebug("Longitud buffer (" + ((byte []) metaInfo["toSHA1info"]).Length.ToString()+") Longitud hash (" +  infoHash.Length.ToString() + ")");
			}
			catch (DictionaryException de) {
				Debugging.StringToDebug("Error: " + de.Message);
				announce = null;
				info = null;
			}
		}

		/*public object Info(string key) {
			return this.info[key];
		}*/

		#region Properties

		public string TrackerUrl {
			//get { return (string) metaInfo["announce"]; }
			get { return ( (Types.String) metaInfo["announce"]).ToString(); }
		}

		public string InfoHash {
			get { return HexEncoding.ToString(infoHash).ToLower(); } // + " (" + infoHash.Length.ToString() + ")"; }
		}

		public byte [] CodedInfoHash {
			get { return infoHash; }
		}													 

		public InfoDictionary Info {
			get { return info; }
		}

		#endregion
	}
}
