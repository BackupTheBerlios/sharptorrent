using System;
using System.Text;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	/// <summary>
	/// BitTorrent String type.
    /// Strings are length-prefixed base ten followed by a colon and the string. 
    /// For example 4:spam  corresponds to 'spam'.
    /// </summary>
    public class StringException : Exception {
		public StringException() : base() {
		}
		public StringException(string message) : base(message) {
		}
        public StringException(string message, Exception innerException) : base(message, innerException) {
        }
    }
    public class String : BeType, IBeType {
		private string theString;

        #region Constructors
        public String() {
		}
		public String(string theString) {
			this.theString = theString;
		}
		public String(BeEncode.String theString) {
            this.theString = theString.StringValue;
        }
        public String(byte[] buffer) :this (buffer, 0, 0) {
        }
        public String(byte[] theBuffer, int pos, int length) {
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
                buffer[i] = theBuffer[pos + i];
            // Check the buffer
            StringBuilder sb = new StringBuilder();
            int index = 0;
            // String length
            while ( (index < buffer.Length) && (buffer[index] != (byte) ':')) 
                sb.Append((char) buffer[index++]);
            int stringLength = 0;
            try {
                stringLength = Int32.Parse(sb.ToString());
            }
            catch (FormatException fe) {
                throw new StringException("Invalid String buffer format. " + fe.Message);
            }
            catch (OverflowException oe) {
                throw new StringException("Invalid String buffer format. " + oe.Message);
            }
            // Reset the container
            sb.Length = 0;
            // Remove the :
            index++;
            while (index < buffer.Length) {
                sb.Append((char) buffer[index++]);
            }
            // Check String length
            if (sb.Length != stringLength)
                throw new StringException("Invalid String buffer format. String Length error.");
            theString = sb.ToString();
        }
        #endregion
        #region Properties
        public string StringValue {
            get { return theString; }
        }
        #endregion

        #region IBeType Members

        public void Set(int value) {
            theString = value.ToString();
        }
        
        public void Set(string value) {
            theString = value;
        }

        public byte [] BeEncode() {
            int stringLength = theString.Length.ToString().Length;
            int index = 0;
            beEncoded = new byte[theString.Length + stringLength + 1];
            for (; index < stringLength; index++)
                beEncoded[index] = (byte) stringLength.ToString()[index];
            beEncoded[index++] = (byte)':';
            for (int i = 0; i < theString.Length; i++)
                beEncoded[index++] = (byte) theString[i];
            return beEncoded;
        }

        #endregion
    }
}
