#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.Tracker {
  	public class TrackerException : Exception {
		public TrackerException() : base() {
		}
		public TrackerException(string message) : base(message) {
		}
		public TrackerException(string message, Exception innerException) : base(message, innerException) {
		}
    }
    
    public class Tracker {
        public enum TrackerEvents { started, completed, stopped, empty };
        public Tracker() {

        }
    }
}
