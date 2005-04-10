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
using SharpTorrent.BitTorrentProtocol.Utilities;
using SharpTorrent.BitTorrentProtocol.Cryptography;
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
    public delegate void NewTrackerResponse();
    public class Tracker {
        private string urlTracker;
        /// <summary>
        /// TimeOut for the request in milliseconds
        /// </summary>
        private int requestTimeOut = 1 * 60 * 1000;     // 1 minute
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
        private string trackerGetRequest;
        private byte [] trackerResponse;
        private Timer timer;
        private RequestState requestState;
        private bool trackerFailure;
        private string trackerFailureReason;
        /// <summary>
        /// Static properties
        /// </summary>
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        /// <summary>
        /// Events
        /// </summary>
        public event NewPeersEventHandler onNewPeers;
        public event NewTrackerResponse onNewTrackerResponse;

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
            trackerResponse = null;
            timer = null;
        }

        /// <summary>
        /// Create the Tracker class.
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="urlTracker"></param>
        /// <param name="infoHash">This is the Bencoded info value.</param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public Tracker(PeerID peerID, string urlTracker, byte [] infoHash, string ip, int port) : this() {
            this.peerID = peerID;
            this.urlTracker = urlTracker;
            this.infoHash = SHA1.HashValue(infoHash);
            this.ip = ip;
            this.port = port;
        }

        #endregion

        #region Example 
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

        #region Private methods

        private void PrepareTrackerRequest() {
            // Prepare the Get string
            StringBuilder sb = new StringBuilder();
            sb.Append(urlTracker + "?");
            sb.Append("info_hash=" + Conversions.EscapeString(infoHash));
            sb.Append("&peer_id=" + Conversions.EscapeString(peerID.ID));
            sb.Append("&port=" + port.ToString());
            sb.Append("&uploaded=" + uploaded.ToString());
            sb.Append("&downloaded=" + downloaded.ToString());
            sb.Append("&left=" + left.ToString());
            sb.Append("&event=" + status.ToString());
            sb.Append("&num_peers=0");
            sb.Append("&ip=" + ip.ToString());
            // Escape the String
            trackerGetRequest = sb.ToString(); //HttpUtility.HtmlEncode(sb.ToString());
        }
    
        /// <summary>
        ///  This MUST be an asynchronous call
        /// </summary>
        private void SendTrackerGet() {
            PrepareTrackerRequest();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(trackerGetRequest);
            requestState = new RequestState();
            // To store the request
            requestState.request = request;
            try {
                // Start the Async request
                IAsyncResult result = request.BeginGetResponse(new AsyncCallback(EndGetTrackerResponse), requestState);

                // We need a TimeOut
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TrackerGetTimeOut), requestState, requestTimeOut, true);

                // Signal the manual event to wait.
                allDone.WaitOne();
                requestState.response.Close();
                trackerResponse = null;
                trackerFailure = false;
                trackerFailureReason = string.Empty;
            }
            catch(WebException we) {
                throw new TrackerException("Error in GET to tracker. [" + we.Message + "].", we);
            }
        }

        private void EndGetTrackerResponse(IAsyncResult result) {
            requestState = (RequestState) result.AsyncState;
            try {
                WebRequest webRequest = requestState.request;
                requestState.response = webRequest.EndGetResponse(result);

                // Get the response
                requestState.streamResponse = requestState.response.GetResponseStream();

                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader sr = new StreamReader(requestState.streamResponse, encode);
                // Tracker response must have less than 256 bytes.
                char[] bufferRead = new char[512];
                int responseLength = sr.Read(bufferRead, 0, 512);
                trackerResponse = new byte[responseLength];
                for (int i = 0; i < responseLength; i++) {
                    trackerResponse[i] = (byte)bufferRead[i];
                }
                ProcessTrackerResponse();
            }
            catch (WebException we) {
                //throw new TrackerException("Error getting response from tracker. [" + we.Message + "].", we);
            }
            allDone.Set();
        }

        private void TrackerGetTimeOut(object state, bool timedOut) {
            if (timedOut) {
                // Abort the Get
                WebRequest request = (state as RequestState).request;
                if (request != null)
                    request.Abort();
                // To create the timer
                requestInterval = 2 * 60 * 1000;
                ProcessTrackerResponse();
            }
        }

        private void NotifyNewPeerList() {
            if (onNewPeers != null)
                onNewPeers(this);
        }

        private void ProcessTrackerResponse() {
            // The tracker response is a Dictionary
            if (trackerResponse != null) {
                // Create the dictionary
                try {
                    response = new Dictionary(trackerResponse);
                    // Is there a failure reason ??
                    if (response.ContainsKey("failure reason")) {
                        trackerFailure = true;
                        trackerFailureReason = response["failure reason"].ToString();
                    }
                    else {      // We have data from the Tracker
                        // Get the interval
                        requestInterval = ((BeEncode.Integer)response["interval"]).IntegerValue;
                    }
                }
                catch (DictionaryException de) {
                    /// TODO (log error)
                    ;
                }
                catch (IntegerException ie) {
                    /// TODO (log error)
                    ;
                }
                // We have a new Tracker response, notify!!!
                if (onNewTrackerResponse != null)
                    onNewTrackerResponse();
            }
            // Progam the timer to future requests even if not yet completed. Only once
            //if (timer == null)
            //    timer = new Timer(new TimerCallback(TimeToNewRequest), null, requestInterval, requestInterval);
        }

        private void TimeToNewRequest(object state) {
            // Send a new Request
            SendTrackerGet();
        }

        #endregion

        #region Public methods

        public Peers GetPeers() {
            if (trackerResponse != null) {
                return new Peers((BeEncode.Dictionary) response["peers"]);
            }
            else
                throw new TrackerException("There is not a refreshed Peer list.");
        }

        public void StartTrackerRequests() {
            // Start the Tracker requests
            status = TrackerEvents.started;
            SendTrackerGet();
        }

        public void StopTrackerRequests() {
            // Stop the Tracker requests
            status = TrackerEvents.stopped;
            // Stop the timer
            timer.Dispose();
            timer = null;
        }

        #endregion

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

        public int RequestInterval {
            get { return requestInterval; }
        }

        public bool RequestFailure {
            get { return trackerFailure; }
        }

        public string RequestFailureReason {
            get { return trackerFailureReason; }
        }

        public byte[] Sha1InfoHash {
            get { return infoHash; }
            set { infoHash = value; }
        }

        #endregion
    }
}
