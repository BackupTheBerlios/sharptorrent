using System;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'request' messages contain an index, begin, and length. 
	/// The last two are byte offsets. 
	/// Length is generally a power of two unless it gets truncated by the end of the file. 
	/// All current implementations use 2_15, and close connections which request an 
	/// amount greater than 2_17.
	/// </summary>
	public class Request : Message {
		private int index;
		private int begin;
		private int length;

		public Request(int index, int begin, int length) {
			this.type = 6;
			this.index = index;
			this.begin = begin;
			this.length = length;
		}
	}
}
