using System;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Exceptions;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	/// <summary>
	/// BitTorrent String type.
    /// Strings are length-prefixed base ten followed by a colon and the string. 
    /// For example 4:spam  corresponds to 'spam'.
    /// </summary>
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
            // Check the buffer
            StringBuilder sb = new StringBuilder();
            int index = pos;
            // String length
            while ( (index < length) && (theBuffer[index] != (byte) ':')) 
                sb.Append((char) theBuffer[index++]);
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
            if (index >= length)
                throw new StringException("Invalid String buffer format.");
            // Reset the container
            sb.Length = 0;
            // Remove the :
            index++;
            while (index < length) {
                sb.Append((char) theBuffer[index++]);
            }
            // Check String length
            if (sb.Length != stringLength)
                throw new StringException("Invalid String buffer format. String Length error.");
            theString = sb.ToString();
        }
        #endregion

        // Return only the .net string type
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            byte[] beEncode = this.BeEncode();
            // Remove the length
            int actualPos = 0;
            while (beEncode[actualPos] != (char)':')
                actualPos++;
            // Remove ':'
            actualPos++;
            for (int i = actualPos; i < beEncode.Length; i++)
                sb.Append((char)beEncode[i]);
            return sb.ToString();
        }


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
                beEncoded[index] = (byte) theString.Length.ToString()[index];
            beEncoded[index++] = (byte)':';
            for (int i = 0; i < theString.Length; i++)
                beEncoded[index++] = (byte) theString[i];
            return beEncoded;
        }

        #endregion
    }
}
