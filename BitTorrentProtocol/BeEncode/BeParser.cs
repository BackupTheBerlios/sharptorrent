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
        private int nextTokenPos = 1;
        private BeEncode.Dictionary metainfo;
        private string metainfoText;

        public BeParser() {
            metainfo = new BeEncode.Dictionary();
        }

        private BeEncode.String ParseString() {

        }

        private BeEncode.Integer ParseString() {

        }

        private BeEncode.List ParseString() {

        }

        private BeEncode.Dictionary ParseString() {

        }

        public BeEncode.Dictionary Parse(string Text) {
            metainfoText = Text;
            // The begining is a 'd'
            if ((char)metainfoText[actualTokenPos] != 'd')
                throw new BeParserException("This is not a valid dictionary.");
            
            return metainfo;
        }
    }
}
