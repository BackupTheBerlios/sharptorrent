using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Types;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	public class PeersException : Exception {
		public PeersException() : base() {
		}
		public PeersException(string message) : base(message) {
		}
		public PeersException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// This represents a list of Peers.
	/// </summary>
	public class Peers {
		private Hashtable peers = new Hashtable();

		public Peers() {
		}

		public void Add(string ip, PeerID id, Int32 port) {
			Peer peer = new Peer(ip, id, port);
			peers.Add(id.ToString(), peer);
		}

		public void Add(Peer peer) {
			peers.Add(peer.Id, peer);
		}

		public bool ContainsKey(string key) {
			return peers.ContainsKey(key);
		}

		public void Remove(PeerID key) {
			if (peers.ContainsKey(key))
				peers.Remove(key);
		}

		public IDictionaryEnumerator GetEnumerator() {
			return peers.GetEnumerator();
		}

		#region Properties
		
		public Peer this[PeerID key] {
			get { 
				if (peers.ContainsKey(key))
					return (Peer) peers[key];
				else
					throw new PeersException("The (" + key + ") doesn't exists.");
			}
		}

		/*public Peer this[Int32 index] {
			get {
				if (peers.Count >= index)
					return (Peer) peers[index];
				else
					throw new PeersException("The (" + index.ToString() + ") element doesn't exists.");
			}
		}*/

		public Int32 Count {
			get { return peers.Count; }
		}

		#endregion
	}
}
