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
		private Int32 index;
		private Int32 begin;
		private Int32 length;

		public Request(Int32 index, Int32 begin, Int32 length) {
			this.messageType = 6;
			this.index = index;
			this.begin = begin;
			this.length = length;
			DoMessage();
		}

		public Request(MemoryStream stream) {
			this.messageType = 6;
			// Extract the message
			index = BigEndian.FromBigEndian(stream);
			begin = BigEndian.FromBigEndian(stream);
			length = BigEndian.FromBigEndian(stream);
			/* In four byte big-endian
			byte [] buffer = stream.GetBuffer();
			int bufferPos = (int) stream.Position;
			this.index = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, bufferPos));
			stream.Position += 4;
			bufferPos += 4;
			this.begin = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, bufferPos));
			stream.Position += 4;
			bufferPos += 4;
			this.length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, bufferPos));
			stream.Position += 4;*/
		}

		private void DoMessage() {
			// the message
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1 + 3 * BigEndian.BIGENDIANBYTELENGTH];
			StoreMessageLength(3);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
			BigEndian.ToBigEndian(index, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
			BigEndian.ToBigEndian(begin, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
			BigEndian.ToBigEndian(length, ref message, 1 * BigEndian.BIGENDIANBYTELENGTH);
		}
	}
}
