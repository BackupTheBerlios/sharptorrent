using System;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using SharpTorrent.BitTorrentProtocol.Tracker;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	public class DownloaderException : Exception {
		public DownloaderException() : base() {
		}
		public DownloaderException(string message) : base(message) {
		}
		public DownloaderException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// Represent the peer client.
	/// </summary>
	public class Downloader {
		/// <summary>
		/// Const MAXDOWNLOADPEERS is the maximum number of peers to concurrently download from
		/// </summary>
		private const int MAXDOWNLOADPEERS = 1;
		private MetaInfo metaInfoFile;
		private TrackerTalker trackerTalk;
		private Peer us;
		private Peers otherPeers = new Peers();
		private Peers workingPeers = new Peers();
		private Handshake handshake;
		private Thread downloaderThr;
		private bool closeDownloader = false;
		private int maxDownloadPeers = MAXDOWNLOADPEERS;
		/// <summary>
		/// Statics
		/// </summary>
		private int downloaded;
		private int uploaded;
	
		#region Constructors

		public Downloader(string metaInfofile) {
			this.metaInfoFile = new MetaInfo(metaInfofile);
			Dictionary clientHost = new Dictionary(3);
			// Generate an ID
			clientHost.Add("id", new PeerID(GenerateID()));
			// Get the ip
			clientHost.Add("ip", new Types.String("127.0.0.1"));
			// Get the local port
			clientHost.Add("port", new Types.Integer(1331));
			
			// Initialize fields
			us = new Peer((clientHost["ip"]).ToString(), (PeerID) clientHost["id"], ((Types.Integer) clientHost["port"]).ToInt);
			us.OnNewConnection += new NewPeerConnection(Us_OnNewConnection);
			us.OnNewMessage += new NewPeerMessage(Us_OnNewMessage);

			// Contact the tracker
			trackerTalk = new TrackerTalker(metaInfoFile.TrackerUrl, metaInfoFile.CodedInfoHash, clientHost);
			trackerTalk.OnNewPeers += new NewPeers(trackerTalk_OnNewPeers);	

			handshake = new Handshake(metaInfoFile.CodedInfoHash, new PeerID(GenerateID()));

			// Statics
			downloaded = 0;
			uploaded = 0;

			// We can start to talk to tracker
			trackerTalk.StartTrackerTalk();
		}

		#endregion

		private bool CheckHandshake(byte [] handshake) {
			Handshake incomingHandshake = new Handshake(handshake);
			return (incomingHandshake.CompareTo(this.handshake) == true);
		}

		private bool CheckHandshake(byte [] handshake, ref Handshake returnHandshake) {
			Handshake incomingHandshake = new Handshake(handshake);
			returnHandshake = incomingHandshake;
			return (incomingHandshake.CompareTo(this.handshake) == true);
		}

		/// <summary>
		/// There is a new peer asking for connect.
		/// </summary>
		/// <param name="handshake">This is the handshake message</param>
		/// <param name="incoming">The incoming socket</param>
		private void Us_OnNewConnection(byte[] handshake, Socket incoming) {
			// The buffer contains the HandShake, Is it valid ?
			Debugging.StringToDebug("New peer connection", "Downloader");
			// Is it the Handshake valid ??
			Handshake incomingHandshake = null;
			if (CheckHandshake(handshake, ref incomingHandshake)) {
				// Is it on the Tracker peer list  ??
				if (true) { // (otherPeers.ContainsKey(incomingHandshake.Id.ToString())) {
					Debugging.StringToDebug("New peer conected is valid", "Downloader");
					// Create a new peer (incoming)
					Peer incomingPeer = new Peer(incoming);
					incomingPeer.Id = incomingHandshake.Id;
					incomingPeer.Ip = ((IPEndPoint) incoming.RemoteEndPoint).Address.ToString();
					incomingPeer.Port = ((IPEndPoint) incoming.RemoteEndPoint).Port;
					incomingPeer.Send(this.handshake);
					incomingPeer.Send(new Messages.Bitfield());
					incomingPeer.Handshaked = true;
					workingPeers.Add(incomingPeer);
				}
				else
					Debugging.StringToDebug("New peer is not on the peer list", "Downloader");
			}
			else
				Debugging.StringToDebug("New peer has an invalid handshake", "Downloader");
		}

		/// <summary>
		/// Generate an ID. One for each session.
		/// </summary>
		/// <returns>A 20 byte id.</returns>
		private byte [] GenerateID() {
			//Random rdn = new Random();
			byte [] id = new byte [20];
			//rdn.NextBytes(id);
			id[0] = (byte) 'S';
			id[1] = (byte) 'T';
			id[2] = (byte) '-';
			for (int i = 3; i < 20; i++)
				id[i] = (byte) 'S';
			return id;
		}

		private void Process() {
			while (!closeDownloader) {
				// Check our Peer
				if (us.DataAvailable())
					us.RecieveData();
				
				// Are we working with the maximum peers ?
				if ( (maxDownloadPeers > workingPeers.Count) && (otherPeers.Count > 0) ) {
					// Look for the first peer not in Working Status
					Peer newPeer = null;
					IDictionaryEnumerator keys = otherPeers.GetEnumerator();
					while (keys.MoveNext()) {
						Peer tryPeer = ((Peer) otherPeers[(PeerID) keys.Key]);
						if ((tryPeer.ActualPeerStatus != PeerStatus.PSWorkingWith) && (tryPeer.ActualPeerStatus != PeerStatus.PSBadPeer)) {
							newPeer = (Peer) otherPeers[(PeerID) keys.Key];
							break;
						}
					}
					/* Got a peer
					if (newPeer != null) {
						Debugging.StringToDebug("We have a new peer to work with." , "Downloader");
						try {
							newPeer.OnNewConnect += new NewConnect(newPeer_OnNewConnect);
							newPeer.Connect(handshake);
							workingPeers.Add(newPeer);
						}
						catch (PeerCommunicationsException pce) {
							Debugging.StringToDebug("Error conectado a un nuevo Peer. (" + pce.Message + ")", "Downloader");
							// We know this is a bad peer
							newPeer.Disconnect(true);
						}
					}*/
				}
					
				// Is there data available or anyone unchonking us?
				IDictionaryEnumerator workingKeys = workingPeers.GetEnumerator();
				while (workingKeys.MoveNext())	{
					// Get the peer
					Peer actual = (Peer) workingPeers[(PeerID) workingKeys.Key];
					// Unchoking
					if (actual.ChokedState == ChokedPeerState.Choked) {
						Debugging.StringToDebug("This peer is Choked", "Downloader");
					}
					if (actual.InterestedState == InterestedPeerState.NotInterested) {
						Debugging.StringToDebug("This peer is NOT interested", "Downloader");
					}
					bool test=false;
					if (test) {
						int test2 = 1;
						switch (test2) {
							case 1: actual.Send(new Messages.UnChoke());
								break;
							case 2: actual.Send(new Messages.Choke());
								break;
							case 3: actual.Send(new Messages.Interested());
								break;
							case 4: actual.Send(new Messages.UnInterested());
								break;
						}
					}
					// Data available
					if (actual.DataAvailable()) {
						actual.RecieveData();
					}
				}

				Thread.Sleep(2000);
			}
			Debugging.StringToDebug("Cerrando Thread que gestiona las conexiones.", "Downloader");			
		}

		private void trackerTalk_OnNewPeers() {
			// QUE PASA SI TENEMOS UN PEER EN LA LISTA QUE YA NO ESTA EN EL TRACKER ????
			Peers peers = trackerTalk.GetPeers;
			Debugging.StringToDebug("There are " + trackerTalk.PeersCount.ToString() + " with ourselves.");
			foreach (object obj in peers) {
				Peer peer = (Peer) ((DictionaryEntry) obj).Value;
				if (!otherPeers.ContainsKey(peer.Id.ToString()) && (peer.Id != us.Id)) {
					Peer newPeer = new Peer(peer);
					newPeer.OnNewMessage += new NewPeerMessage(Us_OnNewMessage);
					otherPeers.Add(newPeer);
				}
			}
		}

		private void CloseConnections() {
			try {
				// Stop Us
				us.Disconnect();
				IDictionaryEnumerator keys = workingPeers.GetEnumerator();
				if (keys.MoveNext()) {
					do {
						Peer peer = (Peer) workingPeers[(PeerID) keys.Key];
						peer.Disconnect();
						workingPeers.Remove((PeerID) keys.Key);
						keys = workingPeers.GetEnumerator();
					} while (keys.MoveNext());
				}
			}
			catch (PeerCommunicationsException pce) {
				Debugging.StringToDebug("Error cerrando comunicaciones. (" + pce.Message + ")", "Downloader");
			}
		}

		private void SendMessage() {
			/*IDictionaryEnumerator keys = rights.GetEnumerator();
			// while (keys.MoveNext()) {
			keys.MoveNext();
			RightPeer peer = (RightPeer) rights[(PeerID) keys.Key];
			if (peer.Handshaked) {
				peer.SendMessage(new Messages.Request(0, 0, 16));
			}*/
		}

		private void Us_OnNewMessage(PeerID id, Message message) {
			Debugging.StringToDebug("New message from peer, [MessageType=" + message.GetType() + "]", "Downloader");
		}

		public void StartDownload() {
			/* Start to listen */
			try {
				us.Listen();
			}
			catch (PeerCommunicationsException pce) {
				Debugging.StringToDebug("Error colocando peer en modo de escucha. (" + pce.Message + ")", "Downloader");
			}
			// Process the peers
			downloaderThr = new Thread(new ThreadStart(Process));
			Debugging.StringToDebug("Creado el Thread downloader", "Thread");
			downloaderThr.Start();
		}

		public void StopDownload() {
			if (!closeDownloader) {
				trackerTalk.StopTrackerTalk();
				CloseConnections();
				closeDownloader = true;
				downloaderThr.Join();
			}
		}

		public void AskTracker() {
			trackerTalk.AskTracker();
		}

		#region Properties

		public int Downloaded {
			get { return downloaded; }
		}

		public int Uploaded {
			get { return uploaded; }
		}

		public int MaxDownloadPeers {
			set { maxDownloadPeers = value; }
			get { return maxDownloadPeers; }
		}

		#endregion

		private void newPeer_OnNewConnect(byte[] buffer) {
			// We are connected to a new Peer
			if (!CheckHandshake(buffer)) {
				// Not a valid peer
				Debugging.StringToDebug("The new connected peer is not valid.", "Downloader");
				// Disconnect
				throw new PeerCommunicationsException("The peer is not valid.");
			}
			else
				Debugging.StringToDebug("Connected to a new peer.", "Downloader");
		}
	}
}
