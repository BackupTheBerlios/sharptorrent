using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// 0 - choke 
	/// 1 - unchoke 
	/// 2 - interested 
	/// 3 - not interested 
	/// 
	/// 'choke', 'unchoke', 'interested', and 'not interested' have no payload.
	/// </summary>
	public abstract class StateMessages : Message {
		public StateMessages() {
			message = new byte [BigEndian.BIGENDIANBYTELENGTH + 1];
		}
	}

	public class Choke : StateMessages {
		public Choke() {
			messageType = 0;
			StoreMessageLength(1);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
		}
	}

	public class UnChoke : StateMessages {
		public UnChoke() {
			messageType = 1;
			StoreMessageLength(1);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
		}
	}

	public class Interested : StateMessages {
		public Interested() {
			messageType = 2;
			StoreMessageLength(1);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
		}
	}

	public class UnInterested : StateMessages {
		public UnInterested() {
			messageType = 3;
			StoreMessageLength(1);
			message[BigEndian.BIGENDIANBYTELENGTH] = messageType;
		}
	}
}
