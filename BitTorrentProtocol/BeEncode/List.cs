using System;
using System.Collections;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	public class ListException : Exception {
		public ListException() : base() {
		}
		public ListException(string message) : base(message) {
		}
		public ListException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	
	/// <summary>
	/// Represents a BitTorrent List.
    ///  Lists are encoded as an 'l' followed by their elements (also bencoded) followed by an 'e'. For example l4:spam4:eggse  corresponds to ['spam', 'eggs'].
    /// </summary>
	public class List {
		private ArrayList elements;

		public List() :this(0) {
		}

		public List(int numElements) {
			if (numElements > 0)
				elements = new ArrayList(numElements);
			else
				elements = new ArrayList();
		}

	}
}
