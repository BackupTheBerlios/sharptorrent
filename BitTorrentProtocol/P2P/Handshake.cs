using System;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// The handshake starts with character ninteen (decimal) followed by the string 
	/// 'BitTorrent protocol'. The leading character is a length prefix, put there in the 
	/// hope that other new protocols may do the same and thus be trivially distinguishable 
	/// from each other.
	/// 
	/// After the fixed headers come eight reserved bytes, which are all zero in all current 
	/// implementations. If you wish to extend the protocol using these bytes, please 
	/// coordinate with Bram Cohen to make sure all extensions are done compatibly.
	/// 
	/// Next comes the 20 byte sha1 hash of the bencoded form of the info value from the 
	/// metainfo file. (This is the same value which is announced as info_hash to the tracker,
	/// only here it's raw instead of quoted here). If both sides don't send the same value, 
	/// they sever the connection. The one possible exception is if a downloader wants to do
	/// multiple downloads over a single port, they may wait for incoming connections to 
	/// give a download hash first, and respond with the same one if it's in their list.
	/// 
	/// After the download hash comes the 20-byte peer id which is reported in tracker 
	/// requests and contained in peer lists in tracker responses. If the receiving 
	/// side's peer id doesn't match the one the initiating side expects, it severs the 
	/// connection.
	/// </summary>
	public class Handshake : Message {
		public const int HANDSHAKEHEADSIZE = 20;
		public const int HANDSHAKERESERVEDSIZE = 8;
		public const int HANDSHAKESIZE = 40; //HANDSHAKEHEADSIZE + HANDSHAKERESERVEDSIZE + SHA1.SHA1SIZE + string[20].string[20]SIZE;
		private byte [] bitTorrentHead = new byte[HANDSHAKEHEADSIZE];
		//private string[20] id;
		//private byte [] sha1;

		public Handshake() {
		}
		/// <summary>
		/// With this constructor we can obtain the incoming handshake
		/// </summary>
		/// <param name="buffer"></param>
		public Handshake(byte [] buffer) {
		/*	int indBuffer = 0;
			// Protocol
			for (int indP = 0; indP < HANDSHAKEHEADSIZE; indP++) 
				bitTorrentHead[indP] = buffer[indBuffer++];
			// Reserved
			for (int indR = 0; indR < HANDSHAKERESERVEDSIZE; indR++)
				indBuffer++;
			// sha1
			sha1 = new byte [SHA1.SHA1SIZE];
			for (int indS = 0; indS < SHA1.SHA1SIZE; indS++)
				sha1[indS] = buffer[indBuffer++];
			// id
			byte [] theId = new byte [string[20].string[20]SIZE];
			for (int indId = 0; indId < string[20].string[20]SIZE; indId++)
				theId[indId] = buffer[indBuffer++];
			id = new string[20](theId);*/
		}

		/// <summary>
		/// Example (Hex values and byte values):
		///
		/// Hex values:
		/// 19BitTorrrent Protocol = 13 42 69 74 54 6F 72 72 65 6E 74 20 70 72 6F 74 6F 63 6F 6C
		/// 8 Reserved bytes = 00 00 00 00 00 00 00 00
		/// SHA1 = 28 CE 85 2A 75 01 E2 86 33 F2 B8 07 95 26 8F B3 BC 41 92 09
		/// ID =   2D 41 5A 32 30 36 30 2D 61 67 44 4E 69 75 78 78 41 7A 58 39
		///
		/// QUITAR
		/// 		Received: 13426974546F7272656E742070726F746F636F6C
		/// 		0000000000000000
		/// 		28CE852A7501E28633F2B80795268FB3BC419209
		/// 		2D415A323036302D643437593075395A42773576
		/// 		00 00 00 0D 05 FF FF FF FF FF FF FF FF FF FF FF E0
		///byte values: 196610511684111114114101110116321121141111161119911110800000000402061334211712261345124218471493814317918865146945659050485448451005255894811757906611953118000135255255255255255255255255255255255224
		/// QUITAR
		///
		/// Byte values:
		/// 19BitTorrent protocol =
		/// byte values: 1966105116841111141141011101163211211411111611199111108
		/// 8 Reserved bytes = 00000000
		/// SHA1 = 40 206 133 42 117 1 226 134 51 242 184 7 149 38 143 179 188 65 146 9
		/// ID = 456590504854484573897211065869775666969103
		/// </summary>
		
		#region Properties

		#endregion
	}
}
