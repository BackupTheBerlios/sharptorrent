using System;
using System.Net;
using System.IO;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.P2P;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Tracker {
	/// <summary>
	/// This is who actually talks to the Tracker.
	/// </summary>
	public delegate void NewPeers();
	public class TrackerTalker {
		private Request theRequest = new Request();
		private Response theResponse = new Response();
		private HttpWebRequest request;
		private WebResponse response;
		private System.Timers.Timer tmr = new System.Timers.Timer(30000);
		private bool Stopped = false;
		public event NewPeers OnNewPeers;

		public TrackerTalker(string announce, byte [] infoSHA1, Types.Dictionary clientHost) {
			tmr.Enabled = false;
			tmr.AutoReset = false;
			tmr.Elapsed += new System.Timers.ElapsedEventHandler(TimeToQueryTracker);
			theRequest.TrackerUrl = announce;
			theRequest.InfoHash = infoSHA1;
			theRequest.PeerId = (PeerID) clientHost["id"];
			theRequest.Ip = ((Types.String) clientHost["ip"]).ToString();
			theRequest.Port = ((Types.Integer) clientHost["port"]).ToInt;
			theRequest.Downloaded = 0;
			theRequest.Uploaded = 0;
      theRequest.Left = 0;
			theRequest.ActualState = Enumerations.Events.started;
		}

		/// <summary>
		/// http://localhost:7171/announce
		/// ?info_hash=%AEx%F6%F3%BA%23l%D7%A7%5B%13%2B%0A%22r%C8%1E%AA%3C%9D
		/// &peer_id=S587-----b3LXZ9T2-fr
		/// &port=6884&ip=127.0.0.1&uploaded=0&downloaded=0&left=0&event=stopped
		/// </summary>
		private void SendGetToTracker() {
			if (Stopped)
				return;
			request = (HttpWebRequest) WebRequest.Create(theRequest.GetRequest);
			try {
				try {
					response = request.GetResponse();
					Stream receiveStream = response.GetResponseStream();
					Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
					StreamReader sr = new StreamReader(receiveStream, encode);
					char [] bufferRead = new char [256];
					Int32 responseLength = sr.Read(bufferRead, 0, 256);
					byte [] buffer = new byte [responseLength];
					System.Diagnostics.Debug.Write("Devuelto:");
					for (int i = 0; i < responseLength; i++) {
						buffer[i] = (byte) bufferRead[i];
						System.Diagnostics.Debug.Write(bufferRead[i]);
					}
					Debugging.StringToDebug("");
					theResponse.ProcessResponse(buffer);
					// It can fail
					if (!theResponse.Failed) {
						tmr.Interval = theResponse.Interval;
						// There are new peers, raise an event
						if (OnNewPeers != null) 
							OnNewPeers();
					}
				}
				catch (WebException we) {
					throw new TrackerException("Petición incorrecta. (" + we.Message + ")");
				}
			}
			finally {
				tmr.Enabled = true;
			}
		}	
	
		private void TimeToQueryTracker(object sender, System.Timers.ElapsedEventArgs e) {
			tmr.Enabled = false;
			SendGetToTracker();
		}

		public void StartTrackerTalk() {
			// Start talking to the Tracker
			SendGetToTracker();
		}

		public void StopTrackerTalk() {
			Stopped = false;
			tmr.Enabled = false;
		}

		public void AskTracker() {
			SendGetToTracker();
		}

		#region Properties

		public bool TrackerWorking {
			get { return !theResponse.Failed; }
		}

		public string TrackerResponse {
			get { 
				if (theResponse.Failed)
					return theResponse.FailureReason;
				else
					return "Ok...";
			}
		}

		public Peers GetPeers {
			get { return theResponse.TrackerPeers; }
		}

		public Int32 PeersCount {
			get { return theResponse.PeersCount; }
		}

		#endregion
	
	}
}
