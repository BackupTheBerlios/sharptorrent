#region Using directives

using System;
using System.Collections;
#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P {
    /// <summary>
    /// This is a <see>Connection</see> list.
    /// </summary>
    public class Connections : IEnumerable, IEnumerator {
        private ArrayList connections;
        private int index = -1;

        public Connections() : this(0) {
        }

        public Connections(int numElements) {
            if (numElements > 0)
                connections = new ArrayList(numElements);
            else
                connections = new ArrayList();
        }

        public void Add(Connection newConnection) {
            connections.Add(newConnection);
        }

        public void Add(Connections connections) {
            foreach (Connections connection in connections)
                this.Add(connection);
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        #endregion

        # region Propiedades
        public int ConnectionsNumber {
            get { return connections.Count; }
        }
       
        public Connection this [int index] {
            get {
                if ((index <= 0) || (index > connections.Count))
                    throw new System.InvalidOperationException("Index out of bounds.");
                else
                    return (Connection) connections[index]; 
            }
        }
        #endregion
        #region IEnumerator Members
        public object Current {
            get {
                if ((index < 0) || (index >= connections.Count))
                    throw new System.InvalidOperationException("Index before/after elements on container.");
                else
                    return connections[index];
            }
        }

        public bool MoveNext() {
            index++;
            return (index < connections.Count);
        }

        public void Reset() {
            index = -1;
        }
        #endregion
    }
}
