using System;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.Utilities;
using SharpTorrent.BitTorrentProtocol.P2P;

namespace SharpTorrent.BitTorrentProtocol.Tracker {
	/// <summary>
	/// Tracker responses are bencoded dictionaries.
	/// If a tracker response has a key <b>failure reason</b>, then that maps to a human 
	/// readable string which explains why the query failed, and no other keys are required. 
	/// Otherwise, it must have two keys: 
	///		<b>interval</b>, which maps to the number of seconds the downloader should wait 
	///		between regular rerequests.
	///		<b>peers</b>, which maps to a list of dictionaries corresponding to peers, each of 
	///		which contains the keys <b>peer</b> <b>id</b>, <b>ip</b>, and <b>port</b>, which 
	///		map to the peer's self-selected ID, IP address or dns name as a string, 
	///		and port number, respectively. 
	///		
	///		Note that downloaders may rerequest on nonscheduled times if an event happens or 
	///		they need more peers.
	/// </summary>
	public class Response {
		/// <summary>
		/// This is where the response will be
		/// </summary>
		private Dictionary response;
		/// <summary>
		/// The Get request failed ?
		/// </summary>
		private bool failed;
		/// <summary>
		/// If the Get request failed, why ?
		/// </summary>
		private string failure_reason;
		/// <summary>
		/// Number of seconds to wait between another request
		/// </summary>
		private Int32 interval;
		/// <summary>
		/// List of peers
		/// </summary>
		private Peers peers = new Peers();

		public Response() {
		}

		public void ProcessResponse(byte [] buffer) {
			BeParser toParse = new BeParser(buffer);
			try {
				response = Dictionary.Decode(toParse);
			}
			catch (DictionaryException de) {
				failed = true;
				failure_reason = "The response has not a dictionary. (" + de.Message + ")";
				return;
			}
			if (response.ContainsKey("failure reason")) {
				failed = true;
				failure_reason = ((Types.String) response["failure reason"]).ToString();
			}
			else {
				// number of seconds
				interval = ((Types.Integer) response["interval"]).ToInt * 1000;
				// List of peers
				Types.List responsePeers = (Types.List) response["peers"];
				foreach (Types.Dictionary peer in responsePeers) {
					string ip = ((Types.String) peer["ip"]).ToString();
					PeerID id = new PeerID(((Types.String) peer["peer id"]).ToString());
					Int32 port = ((Types.Integer) peer["port"]).ToInt;
					/// MIRAR
					if (!peers.ContainsKey(id.ToString()))
						peers.Add(ip, id, port);
				}
			}
		}

		#region Properties

		public bool Failed {
			get { return failed; }
		}

		public string FailureReason {
			get { 
				if (failed)
					return failure_reason;
				else
					return null;
			}
		}

		public Int32 PeersCount {
			get { return peers.Count; }
		}

		public Peers TrackerPeers {
			get { return peers; }
		}

		public Int32 Interval {
			get { return interval; }
		}

		#endregion
	}
}
