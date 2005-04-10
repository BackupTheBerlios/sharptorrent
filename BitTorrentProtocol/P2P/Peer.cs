using System;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P {

    #region PeerID class
    /// <summary>
    /// This represent the Peer ID, a random 20 bytes length.
    /// </summary>
    public class PeerID {
        /// <summary>
        /// The Peer ID.
        /// </summary>
        private byte[] peerID;

        #region Constructors

        /// <summary>
        /// Creates a new RandomID.
        /// </summary>
        public PeerID() {
            string sharpTorrent = "SharpTorrent-";
            int index = 0;

            peerID = new byte[20];
            foreach (byte letter in sharpTorrent)
                peerID[index++] = letter;
            // Random ID
            for (; index < 20; index++)
                /// TODO: Generar random byte
                peerID[index] = (byte)'A'; 
        }
        
        /// <summary>
        /// Create a PeerID from another Peer.
        /// </summary>
        /// <param name="peerID">The source Peer ID</param>
        public PeerID(PeerID peerID) {
            this.peerID = new byte[20];
            // Copy Id
            this.peerID = (byte[])peerID.peerID.Clone();
        }

        #endregion

        #region Public Methods

        public override string ToString() {
            // Return the (20 byte) ID as a string
            return Conversions.ConvertByteArrayToString(peerID);
        }

        #endregion

        #region Properties

        public byte[] ID {
            get { return this.peerID; }
        }
        
        #endregion

    }
    
    #endregion

    /// <summary>
    /// Those are the peer Status.
    /// </summary>
    public enum PeerStatus { psInitializing, psContacted, psWorking, psDisconnected };

    #region Peer class

    /// <summary>
    /// <b>peerID</b>, random 20 length as a string.
    /// <b>peerIP</b>, address or dns name as a string. 
    ///	<b>peerPort</b>, port number of the peer.
    /// 
    ///	Peer connections are symmetrical. Messages sent in both directions look the same, 
    ///	and data can flow in either direction.
    ///	
    ///	When a peer finishes downloading a piece and checks that the hash matches, it 
    ///	announces that it has that piece to all of its peers.
    ///	
    ///	Connections contain two bits of state on either end: <b>choked</b> or <b>not</b>, 
    ///	and <b>interested</b> or <b>not</b>. 
    ///	Choking is a notification that no data will be sent until unchoking happens.
    ///	
    /// </summary>
    public class Peer {
        private PeerID peerID;
        private string peerIP;
        private int peerPort;

        #region Constructors
        public Peer() {
            peerID = new PeerID();
            peerIP = "192.168.1.2";
            peerPort = 6968;
        }       
        #endregion
        #region Properties
        public byte [] PeerID {
            get { return peerID.ID; }
        }

        public string PeerIP {
            get { return peerIP; }
        }

        public int PeerPort {
            get { return peerPort; }
        }
        #endregion
    }

    #endregion
}
