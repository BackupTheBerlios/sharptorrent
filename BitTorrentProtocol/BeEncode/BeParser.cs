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
        private int actualTokenPos = 0;
        private BeEncode.Dictionary metainfo;
        
        public BeParser() {
            metainfo = new BeEncode.Dictionary();
        }

        private BeEncode.String ParseString() {

        }

        private BeEncode.Integer ParseInteger() {

        }

        private BeEncode.List ParseList() {

        }

        private BeEncode.Dictionary ParseDictionary() {

        }

        public BeEncode.Dictionary Parse(byte [] buffer) {
            // This MUST be a Dictionary
            if ((char)buffer[actualTokenPos++] != 'd')
                throw new BeParserException("This is not a valid dictionary.");
            // Create de Dictionary
            metainfo = new Dictionary();
            while (actualTokenPos < buffer.Length) {
                
                
            }
            // There must be a 'e' at the end
            if ((char) metainfo[actualTokenPos] != 'e')
                throw new BeParserException("This is not a valid dictionary.");
            return metainfo;
        }
    }
}
