using System;

namespace SharpTorrent.BitTorrentProtocol.Tracker {
	/// <summary>
	/// Tracker Exception
	/// </summary>
	public class TrackerException : Exception {
		public TrackerException() : base() {
		}
		public TrackerException(string message) : base(message) {
		}
		public TrackerException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}
