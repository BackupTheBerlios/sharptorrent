using System;
using System.Collections;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	/// <summary>
	/// Dictionary Exception
	/// </summary>
	public class DictionaryException : Exception {
		public DictionaryException() : base() {
		}
		public DictionaryException(string message) : base(message) {
		}
		public DictionaryException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// Represents a BitTorrent dictionarie.
	/// </summary>
	public class Dictionary {
		protected Hashtable elements;

		public Dictionary() {
		}
	}
}
