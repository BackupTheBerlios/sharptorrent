#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
    public class BeParserException : Exception {
    	public BeParserException() : base() {
		}
		public BeParserException(string message) : base(message) {
		}
        public BeParserException(string message, Exception innerException) : base(message, innerException) {
        }
    }
    public class BeParser {
        public BeParser() {
        }

        public BeEncode.Dictionary Parse() {
            return null;
        }
    }
}
