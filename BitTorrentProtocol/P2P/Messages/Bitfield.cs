using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 'bitfield' is only ever sent as the first message. 
	/// Its payload is a bitfield with each index that downloader has sent set to one 
	/// and the rest set to zero. 
	/// 
  ///	Downloaders which don't have anything yet may skip the 'bitfield' message. 
  ///	The first byte of the bitfield corresponds to indices 0 - 7 from high bit to low bit,
  ///	respectively. The next one 8-15, etc. Spare bits at the end are set to zero.
	/// </summary>
	public class Bitfield : Message {
		private BitArray Indexs;
		
		public Bitfield(BitArray Indexs) {
			messageType = 5;
			this.Indexs = Indexs;
		}
	
		public Bitfield(byte [] BitField) {
			messageType = 5;
			Indexs = new BitArray(BitField);
		}

		public Bitfield(Bitfield BitField) {
			messageType = 5;
			Indexs = new BitArray(BitField.Indexs);
		}

/* DEBUG
 * 
 * 0 0 0 13 5 255 255 255 255 255 255 255 255 255 255 255 224
 */
		public Bitfield() {
			message = new byte[17];
			message[0] = 0; message[1] = 0; message[2] = 0;
			message[3] = 13;
			message[4] = 5;
			for (int ind = 5; ind < 16; ind++)
				message[ind] = 255;
			message[16] = 224;
		}

		/* ENDDEBUG 
		 */
		private void DoMessage() {
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1 + Indexs.Count];
			StoreMessageLength(1 + Indexs.Count);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
			//Indexs.
		}

		#region Properties

		public bool this [int idx ] {
			get { return Indexs[idx]; }
		}

		#endregion
	}
}
