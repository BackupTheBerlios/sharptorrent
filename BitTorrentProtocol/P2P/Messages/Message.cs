using System;
using System.Net;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// Represents a Peer2Peer message.
	/// 
	/// After Handshake all later integers sent in the protocol are encoded as 
	/// four bytes big-endian.
	/// 
	/// Messages comes as alternating stream of length prefixes and messages. 
	/// Messages of length zero are keepalives, and ignored. 
	/// 
	/// All non-keepalive messages start with a single byte which gives their type. 
	/// The possible values are:
	/// 
	/// 0 - choke 
	/// 1 - unchoke 
	/// 2 - interested 
	/// 3 - not interested 
	/// 4 - have 
	/// 5 - bitfield 
	/// 6 - request 
	/// 7 - piece 
	/// 8 - cancel 
	/// 
	/// 'choke', 'unchoke', 'interested', and 'not interested' have no payload.
	/// </summary>
	public abstract class Message {
		protected byte messageType;
		protected byte [] message;

		public Message() {
		}

		public virtual byte [] ByteMessage() {
			return message;
		}

		public new string ToString() {
			return HexEncoding.ToString(message);
		}

		protected virtual void StoreMessageLength(int Length) {
			BigEndian.ToBigEndian(Length, ref message, 0);
		}
	}
}
