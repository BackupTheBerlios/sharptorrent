using System;
using System.IO;
using System.Text;
using System.Web;
using SharpTorrent.BitTorrentProtocol.Types;
//using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Tracker {
	/// <summary>
	/// This a GET Request to the Tracker on the web server
	/// 
	/// </summary>
	public class Request {
		/// <summary>
		/// This is the Request to send using a Get web server message
		/// 
		/// info_hash
		/// peer_id (optional)
		/// ip (opional)
		/// port
		/// uploaded
		/// downloaded
		/// left
		/// event
		///		%28%CE%85*u%01%E2%863%F2%B8%07%95%26%8F%B3%BCA%92%09
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
		/// </summary>

		private string trackerUrl;
		/// <summary>
		/// The 20 byte sha1 hash of the bencoded form of the info value from the metainfo file. 
		/// Note that this is a substring of the metainfo file. 
		/// This value will almost certainly have to be escaped.
		/// </summary>
		private byte [] info_hash;
		/// <summary>
		/// A string of length 20 which this downloader uses as its id.
		/// Each downloader generates its own id at random at the start of a new download. 
		/// This value will also almost certainly have to be escaped.
		/// </summary>
		private PeerID peer_id;
		/// <summary>
		/// An optional parameter giving the IP (or dns name) which this peer is at. 
		/// Generally used for the origin if it's on the same machine as the tracker
		/// </summary>
		private string ip;
		/// <summary>
		/// The port number this peer is listening on. Common behavior is for a downloader 
		/// to try to listen on port 6881 and if that port is taken try 6882, then 6883, etc. 
		/// and give up after 6889.
		/// </summary>
		private Int32 port;
		/// <summary>
		/// The total amount uploaded so far, encoded in base ten ascii.
		/// </summary>
		private Int32 uploaded;
		/// <summary>
		/// The total amount downloaded so far, encoded in base ten ascii.
		/// </summary>
		private Int32 downloaded;
		/// <summary>
		/// The number of bytes this peer still has to download, encoded in base ten ascii. 
		/// Note that this can't be computed from downloaded and the file length since it 
		/// might be a resume, and there's a chance that some of the downloaded data failed 
		/// an integrity check and had to be re-downloaded.
		/// </summary>
		private Int32 left;
		/// <summary>
		/// This is an optional key which maps to started, completed, or stopped 
		/// (or empty, which is the same as not being present). 
		/// If not present, this is one of the announcements done at regular intervals. 
		/// An announcement using started is sent when a download first begins, and one using 
		/// completed is sent when the download is complete. 
		/// No completed is sent if the file was complete when started. 
		/// Downloaders send an announcement using 'stopped' when they cease downloading.
		/// </summary>
		private Enumerations.Events actualEvent;

		public Request() {
		}

		/// <summary>
		/// The EscapeString method converts all characters with an ASCII value 
		/// greater than 127 to hexidecimal representation.
		/// </summary>
		/// <param name="str">String to convert</param>
		/// <returns>Escaped string representation of str</returns>
		private string EscapeString(byte [] str) {
			StringWriter sw = new StringWriter();
			foreach (byte chr in str) {
				if ((chr > 127) || (chr < 42))
					sw.Write(Uri.HexEscape((char) chr));
				else
          sw.Write((char)chr);
			}
			sw.Close();
			return sw.ToString();
		}

		#region Properties

		/// <summary>
		/// This must be a get request
		/// Example:
		///		...port=6543&ip=kkdd.dkkd.dikd&...
		/// </summary>
		public string GetRequest {
			get {
				StringWriter sw = new StringWriter();
				sw.Write(trackerUrl+"?info_hash="+EscapeString(InfoHash));
				sw.Write("&peer_id="+EscapeString(peer_id.Id));
				sw.Write("&port="+port.ToString());
				sw.Write("&ip="+ip);
				sw.Write("&uploaded="+uploaded.ToString());
				sw.Write("&downloaded="+downloaded.ToString());
				sw.Write("&left="+left.ToString());
				sw.Write("&event="+actualEvent.ToString());
				sw.Close();
				return sw.ToString();
			}
		}

		public Int32 Port {
			get { return port; }
			set { port = value; }
		}

		public Int32 Uploaded {
			get { return uploaded; }
			set { uploaded = value; }
		}

		public Int32 Downloaded {
			get { return downloaded; }
			set { downloaded = value; }
		}

		public Int32 Left {
			get { return left; }
			set { left = value; }
		}

		public string Ip {
			get { return ip; }
			set { ip = value; }
		}

		public PeerID PeerId {
			get { return peer_id; }
			set { peer_id = value; }
		}

		public byte [] InfoHash {
			get { return info_hash; }
			set { info_hash = value; }
		}

		public Enumerations.Events ActualState {
			get { return actualEvent; }
			set { actualEvent = value; }
		}

		public string TrackerUrl {
			get { return trackerUrl; }
			set { trackerUrl = value; }
		}

		#endregion
	}
}
