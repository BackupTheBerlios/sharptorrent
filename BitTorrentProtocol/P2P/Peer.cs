using System;
using System.Net;
using System.Net.Sockets;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.Tracker;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	/// <summary>
	/// <b>ID</b>, random 20 ength as a string.
	/// <b>IP</b>, address or dns name as a string. 
	///	<b>port</b>, port number of the peer.
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
	public delegate void NewPeerMessage(PeerID id, Message message);
	public enum PeerStatus { PSNotContated, PSContacted, PSWorkingWith, PSBadPeer };
	public class Peer {
		private PeerID id;
		private string ip;
		private Int32 port;
		/// <summary>
		/// If true is Choked
		/// Connections start out Choked
		/// </summary>
		private ChokedPeerState choked = ChokedPeerState.Choked;
		/// <summary>
		/// If true, peer is interested
		/// Connections start out Not Interested
		/// </summary>
		private InterestedPeerState interested = InterestedPeerState.NotInterested;
		/// <summary>
		/// What pieces the peer has availables.
		/// </summary>
		private Messages.Bitfield bitField = null;
		/// <summary>
		/// Did we contact this peer ?
		/// </summary>
		private PeerStatus peerStatus = PeerStatus.PSNotContated;
		private bool handshaked = false;
		/// <summary>
		/// Communications
		/// </summary>
		private PeerCommunications comm; // = new PeerCommunications();
		// Events
		public event NewPeerConnection OnNewConnection;
		public event NewPeerMessage OnNewMessage;
		public event NewConnect OnNewConnect;

		#region Constructors

		public Peer() {
      comm = new PeerCommunications();
			comm.OnNewConnection += new NewPeerConnection(comm_OnNewConnection);
			comm.OnNewMessage += new NewMessage(comm_OnNewMessage);
			comm.OnNewConnect += new NewConnect(comm_OnNewConnect);
		}
		public Peer(string ip, PeerID id, Int32 port) : this() {
			this.id = id;
			this.ip = ip;
			this.port = port;
		}
		public Peer(Peer peer) :this() {
			this.id = peer.id;
			this.ip = peer.ip;
			this.port = peer.port;
		}
		public Peer(Socket peerComm) {
			this.comm = new PeerCommunications(peerComm);
		}
		#endregion

		private void comm_OnNewConnection(byte[] buffer, Socket incoming) {
			if (OnNewConnection != null)
				OnNewConnection(buffer, incoming);
		}

		private void comm_OnNewMessage(Message message) {
			if (message is Messages.Bitfield) {
				this.bitField = new Bitfield((Messages.Bitfield) message);
			}
			if (OnNewMessage != null)
				OnNewMessage(this.id, message);
		}

		public void ChangeInterest() {
			if (interested == InterestedPeerState.NotInterested)
				interested = InterestedPeerState.Interested;
			else
				interested = InterestedPeerState.NotInterested;
		}

		public void ChangeChoke() {
			if (choked == ChokedPeerState.NotChoked)
				choked = ChokedPeerState.Choked;
			else
				choked = ChokedPeerState.NotChoked;
		}

		public void Connect(Handshake handshake) {
			comm.Connect(handshake, IPAddress.Parse("127.0.0.1"), this.port); //this.ip), this.port);
			peerStatus = PeerStatus.PSContacted;
		}

		public void Listen() {
			// Properties
			comm.ListenerPort = this.port;
			comm.ListenerIP = this.ip;
			comm.Listen();
		}

		public void Disconnect() {
			Disconnect(false);
		}

		public void Disconnect(bool badPeer) {
			if (badPeer) 
				peerStatus = PeerStatus.PSBadPeer;
			else
				peerStatus = PeerStatus.PSNotContated;
			comm.Disconnect();
		}

		public void Send(Message message) {
			peerStatus = PeerStatus.PSWorkingWith;
			comm.Send(message);
		}

		public bool DataAvailable() {
			return comm.DataAvailable();
		}

		public void RecieveData() {
			comm.RecieveData();
		}

		#region Properties

		public PeerID Id {
			get { return id; }
			set { id = value; }
		}

		public string Ip {
			get { return ip; }
			set { ip = value; }
		}

		public Int32 Port {
			get { return port; }
			set { port = value; }
		}

		public ChokedPeerState ChokedState {
			get { return choked; }
		}

		public InterestedPeerState InterestedState {
			get { return interested; }
		}

		public IPAddress IpAddress {
			get { return IPAddress.Parse(this.ip); }
		}

		public int Downloaded {
			get { return comm.Downloaded; }
		}

		public int Uploaded {
			get { return comm.Uploaded; }
		}

		public PeerStatus ActualPeerStatus {
			get { return peerStatus; }
		}

		public bool Handshaked {
			set { handshaked = value; }
			get { return handshaked; }
		}

		#endregion

		private void comm_OnNewConnect(byte[] buffer) {
			if (OnNewConnect != null)
				OnNewConnect(buffer);
		}
	}
}
