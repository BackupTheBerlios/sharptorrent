using System;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.FileIO;
using SharpTorrent.BitTorrentProtocol.Tracker;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	/// <summary>
	/// Represent the peer client.
	/// </summary>
	public class ClientPeer : Peer {
		private MetaInfo metaInfoFile;
		private TrackerTalker trackerTalk;
		private Peers bitTorrentPeers = new Peers();

		public ClientPeer(string ip, PeerID id, Int32 port) : base(ip, id, port) {
		}

		/*public ClientPeer(MetaInfo infoFile, Dictionary clientHost) 
				:base( ((Types.String) clientHost["ip"]).ToString(), (PeerID) clientHost["id"], ((Types.Integer) clientHost["port"]).ToInt) {
			this.metaInfoFile = metaInfoFile;
			trackerTalk = new TrackerTalker(metaInfoFile.TrackerUrl, metaInfoFile.CodedInfoHash, clientHost);
			trackerTalk.OnNewPeers +=new NewPeers(trackerTalk_OnNewPeers);
		}*/
		
		public ClientPeer(string metaInfofile) {
			this.metaInfoFile = new MetaInfo(metaInfofile);
			Dictionary clientHost = new Dictionary(3);
			// Generate an ID
			clientHost.Add("id", new PeerID(GenerateID()));
			// Get the ip
			clientHost.Add("ip", new Types.String("127.0.0.1"));
			// Get the local port
			clientHost.Add("port", new Types.Integer(1331));
			
			// Initialize fields
			this.Id = (PeerID) clientHost["id"];
			this.Ip = ((Types.String) clientHost["ip"]).ToString();
			this.Port = ((Types.Integer) clientHost["port"]).ToInt;

			trackerTalk = new TrackerTalker(metaInfoFile.TrackerUrl, metaInfoFile.CodedInfoHash, clientHost);
			trackerTalk.OnNewPeers +=new NewPeers(trackerTalk_OnNewPeers);	
		}

		/// <summary>
		/// Generate an ID. One for each session.
		/// </summary>
		/// <returns>A 20 byte id.</returns>
		private byte [] GenerateID() {
			Random rdn = new Random();
			byte [] id = new byte [20];
			rdn.NextBytes(id);
			id[0] = (byte) 'S';
			id[1] = (byte) 'T';
			id[2] = (byte) '-';
			return id;
		}

		private void trackerTalk_OnNewPeers() {
			foreach (Peer peer in trackerTalk.GetPeers) {
				if (!bitTorrentPeers.ContainsKey(peer.Id.ToString()))
					bitTorrentPeers.Add(peer);
			}
		}


		#region Properties

				

		#endregion
	}
}
