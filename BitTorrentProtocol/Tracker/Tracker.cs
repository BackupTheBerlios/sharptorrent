#region Using directives

using System;
using System.Threading;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Exceptions;
using SharpTorrent.BitTorrentProtocol.BeEncode;
using SharpTorrent.BitTorrentProtocol.P2P;
#endregion

namespace SharpTorrent.BitTorrentProtocol.Tracker {
    /// <summary>
    /// This represents a Tracker.
    /// 
    /// Tracker GET requests have the following keys:
    ///   info_hash 
    ///     The 20 byte sha1 hash of the bencoded form of the info 
    ///     value from the metainfo file. Note that this is a substring of the 
    ///     metainfo file. This value will almost certainly have to be escaped.
    ///   peer_id 
    ///     A string of length 20 which this downloader uses as its id. Each 
    ///     downloader generates its own id at random at the start of a new 
    ///     download. This value will also almost certainly have to be escaped.
    ///   ip 
    ///     An optional parameter giving the IP (or dns name) which this peer is 
    ///     at. Generally used for the origin if it's on the same machine as the 
    ///     tracker.
    ///   port 
    ///     The port number this peer is listening on. Common behavior is for a 
    ///     downloader to try to listen on port 6881 and if that port is taken try
    ///     6882, then 6883, etc. and give up after 6889.
    ///   uploaded 
    ///     The total amount uploaded so far, encoded in base ten ascii.
    ///   downloaded 
    ///     The total amount downloaded so far, encoded in base ten ascii.
    ///   left 
    ///     The number of bytes this peer still has to download, encoded in base 
    ///     ten ascii. Note that this can't be computed from downloaded and the 
    ///     file length since it might be a resume, and there's a chance that some
    ///     of the downloaded data failed an integrity check and had to be 
    ///     re-downloaded.
    ///   event 
    ///     This is an optional key which maps to started, completed, or stopped 
    ///     (or empty, which is the same as not being present). If not present, 
    ///     this is one of the announcements done at regular intervals. An 
    ///     announcement using started is sent when a download first begins, and 
    ///     one using completed is sent when the download is complete. 
    ///     No completed is sent if the file was complete when started. 
    ///     Downloaders send an announcement using 'stopped' when they cease 
    ///     downloading.
    /// 
    /// Tracker responses are bencoded dictionaries. 
    /// 
    ///     If a tracker response has a key failure reason, then that maps to a 
    ///     human readable string which explains why the query failed, and no other
    ///     keys are required. Otherwise, it must have two keys: interval, which 
    ///     maps to the number of seconds the downloader should wait between 
    ///     regular rerequests, and peers. peers maps to a list of dictionaries 
    ///     corresponding to peers, each of which contains the keys peer id, ip, 
    ///     and port, which map to the peer's self-selected ID, IP address or dns 
    ///     name as a string, and port number, respectively. Note that downloaders 
    ///     may rerequest on nonscheduled times if an event happens or they need 
    ///     more peers.
    /// </summary>
    public enum TrackerEvents { started, completed, stopped, empty };
    public delegate void NewPeersEventHandler(Tracker tracker);
    public static ManualResetEvent allDone= new ManualResetEvent(false);
    public class Tracker {
        private string urlTracker;
        /// <summary>
        /// TimeOut for the request in milliseconds
        /// </summary>
        private int requestTimeOut = 30000;
        private Dictionary response;
        private byte[] infoHash;
        private PeerID peerID;
        private string ip;
        private int port;
        private double uploaded;
        private double downloaded;
        private double left;
        private TrackerEvents status;
        private int requestInterval;
        private bool hasTrackerResponse;
        private string trackerGetRequest;
        private byte [] trackerResponse;
        private RequestState requestState;
        /// <summary>
        /// Events
        /// </summary>
        public event NewPeersEventHandler onNewPeers;

        #region Constructors

        public Tracker() {
            urlTracker = string.Empty;
            response = new Dictionary();
            infoHash = null;
            peerID = null;
            ip = string.Empty;
            port = 0;
            uploaded = downloaded = left = 0;
            status = TrackerEvents.empty;
            requestInterval = 0;
            hasTrackerResponse = false;
        }
        public Tracker(PeerID peerID, string urlTracker, byte [] infoHash, string ip, int port) : this() {
            this.peerID = peerID;
            this.urlTracker = urlTracker;
            this.infoHash = infoHash;
            this.ip = ip;
            this.port = port;
        }

        #endregion

        #region Example 
	    /// %28%CE%85*u%01%E2%863%F2%B8%07%95%26%8F%B3%BCA%92%09
		///		This is a real example. (extracted from the Tracker log)
		///		"GET /announce?info_hash=%28%CE%85*u%01%E2%863%F2%B8%07%95%26%8F%B3%BCA%92%09
		///	  &peer_id=-AZ2042-R%40%EB%F4%A2H%A4%2Bx%01%A2c
		///	  &port=6882
		///	  &uploaded=0
		///	  &downloaded=0
		///	  &left=0
		///	  &event=stopped
		///	  &num_peers=0
		///	  &ip=80.24.89.36
        #endregion

        private void PrepareTrackerRequest() {
            // Prepare the Get string
            StringBuilder sb = new StringBuilder();
            sb.Append(urlTracker + "?");
            sb.Append("info_hash=" + infoHash.ToString());
            sb.Append("&peer_id=" + peerID.ToString());
            sb.Append("&port=" + port.ToString());
            sb.Append("&uploaded=" + uploaded.ToString());
            sb.Append("&downloaded=" + downloaded.ToString());
            sb.Append("&left=" + left.ToString());
            sb.Append("&event=" + status.ToString());
            sb.Append("&num_peers=0");
            sb.Append("&ip=" + ip.ToString());
            // Escape the String
            trackerGetRequest = HttpUtility.HtmlEncode(sb.ToString());
        }
    
        /// <summary>
        ///  This MUST be an asynchronous call
        /// </summary>
        private void SendTrackerGet() {
            PrepareTrackerRequest();
            HttpWebRequest request;
            request = (HttpWebRequest) WebRequest.Create(trackerGetRequest);
            requestState = new RequestState();
            // To store the request
            requestState = request;
            // Start the Async request
            IAsyncResult result = request.BeginGetResponse(new AsyncCallback(EndGetResponse), requestState);

            // We need a TimeOut
            ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TrackerGetTimeOut), requestState, requestTimeOut, true);
            

            // Get the response
            response = request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
			Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
			StreamReader sr = new StreamReader(receiveStream, encode);
			// Tracker response must have less than 256 bytes.
            char [] bufferRead = new char [256];
			int responseLength = sr.Read(bufferRead, 0, 256);
			trackerResponse = new byte [responseLength];
			for (int i = 0; i < responseLength; i++) {
                trackerResponse[i] = (byte)bufferRead[i];
            }
        }

        private void EndGetResponse(IAsyncResult result) {

        }

        private void TrackerGetTimeOut(object state, bool timedOut) {
            if (timedOut) {
                // Abort the Get
                HttpWebRequest request = (state as RequestState).request;
                if (request != null)
                    request.Abort();
            }
        }

        private void NotifyNewPeerList() {
            if (onNewPeers != null)
                onNewPeers(this);
        }

        public Peers GetPeers() {
            if (hasTrackerResponse) {
                return null;
            }
            else
                throw new TrackerException("There is not a refreshed Peer list.");
        }

        public void StartTrackerRequests() {
            // Start the Tracker requests
            status = TrackerEvents.started;

        }

        public void StopTrackerRequests() {
            // Stop the Tracker requests
            status = TrackerEvents.stopped;

        }
        #region Properties

        public double Downloaded {
            get { return downloaded; }
            set { downloaded = value; }
        }

        public double Uploaded {
            get { return uploaded; }
            set { uploaded = value; }
        }

        public double Left {
            get { return left; }
            set { left = value; }
        }

        public int RequestTimeOut {
            get { return requestTimeOut; }
            set { requestTimeOut = value; }

        }
        #endregion
    }
}
