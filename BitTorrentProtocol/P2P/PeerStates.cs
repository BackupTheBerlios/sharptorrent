using System;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	/// <summary>
	/// Enumeration with the possible peer states
	/// </summary>
	public enum InterestedPeerState {
		Interested, NotInterested
	}

	public enum ChokedPeerState {
		Choked, NotChoked
	}

}
