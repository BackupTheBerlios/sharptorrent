using System;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	public class DispatcherException : Exception {
		public DispatcherException () : base() {
		}
		public DispatcherException (string message) : base(message) {
		}
		public DispatcherException(string message, Exception innerException) : base(message, innerException) {
		}
	}

    public class Dispatcher {
    
        public Dispatcher() {
        }
    }
}
