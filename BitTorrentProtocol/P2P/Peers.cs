using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.BeEncode;
using SharpTorrent.BitTorrentProtocol.Exceptions;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	/// <summary>
	/// This represents a list of <c>Peer</c> elements.
	/// </summary>
	public class Peers : IEnumerable, IEnumerator {
        private ArrayList peers;
        private int index = -1;

        # region Constructors

        public Peers() : this(0) {
		}
        
        public Peers(int numElements) {
            if (numElements == 0)
                peers = new ArrayList();
            else
                peers = new ArrayList(numElements);
        }

        public Peers(BeEncode.Dictionary peers) {
            try {
                // Create the peers from the dictionary.

            }
            catch (DictionaryException de) {
                /// TODO log error
                
                this.peers = new ArrayList();
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        #endregion
        #region IEnumerator Members
        public object Current {
            get {
                if ((index == -1) || (index >= peers.Count))
                    throw new InvalidOperationException("Index out of bounds.");
                else
                    return peers[index];
            }
        }

        public bool MoveNext() {
            index++;
            return (index < peers.Count);
        }

        public void Reset() {
            index = -1;
        }

#endregion
        #region Properties
        public int PeersNumber {
            get { return peers.Count; }
        }

        public Peer this[int index] {
            get {
                if ((index < 0) || (index >= peers.Count))
                    throw new System.InvalidOperationException("Index before/after elements on container.");
                else
                    return (Peer)peers[index];
            }
        }
        #endregion
    }
}
