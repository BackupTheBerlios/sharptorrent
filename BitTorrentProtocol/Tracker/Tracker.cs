#region Using directives

using System;
using System.Threading;
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
    public class Tracker {
        private string urlTracker;
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
        private bool trackerResponse;
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
            trackerResponse = false;
        }
        public Tracker(PeerID peerID, string urlTracker, byte [] infoHash, string ip, int port) : this() {
            this.peerID = peerID;
            this.urlTracker = urlTracker;
            this.infoHash = infoHash;
            this.ip = ip;
            this.port = port;
        }

        #endregion

        private void NotifyNewPeerList() {
            if (onNewPeers != null)
                onNewPeers(this);
        }

        public Peers GetPeers() {
            if (trackerResponse) {
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

        #endregion
    }
}
