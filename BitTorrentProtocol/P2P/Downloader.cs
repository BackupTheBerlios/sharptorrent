using System;

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

        #region Constructors

        public Downloader() {
        }

        #endregion
    }
}
