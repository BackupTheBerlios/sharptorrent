using System;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Exceptions;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	/// <summary>
	/// BitTorrent integer type.
    ///     Integers are represented by an 'i' followed by the number in base 10 
    ///     followed by an 'e'. For example i3e corresponds to 3 and i-3e 
    ///     corresponds to -3.
    ///     Integers have no size limitation. i-0e is invalid. All encodings with 
    ///     a leading zero, such as i03e, are invalid, other than i0e, which of 
    ///     course corresponds to 0
    /// </summary>
    public class Integer : BeType, IBeType {
		private int theInteger = 0;

        #region Constructors
        public Integer() {
        }
        public Integer(BeEncode.Integer integer) {
            theInteger = integer.IntegerValue;
        }
        public Integer(int integer) {
            theInteger = integer;
        }
        public Integer(string integer) {
            try {
                theInteger = Int32.Parse(integer);
            }
            catch (FormatException fe) {
                throw new IntegerException("Invalid Integer buffer format. " + fe.Message);
            }
            catch (OverflowException oe) {
                throw new IntegerException("Invalid Integer buffer format. " + oe.Message);
            }
        }
        public Integer(byte[] buffer) : this(buffer, 0, 0) {
        }
        public Integer(byte[] theBuffer, int pos, int length) {
            // Check the Buffer
            if (theBuffer[pos] != (byte) 'i')
                throw new IntegerException("Invalid Integer buffer format.");
            if ( (theBuffer[pos + 1] == (byte) '0') && (theBuffer[pos + 2] != (byte) 'e'))
                throw new IntegerException("Invalid Integer buffer format. The Integer starts with 0.");
            // Holds the number
            StringBuilder sb = new StringBuilder();
            for (int i = pos + 1; i <= length; i++)
                sb.Append((char)theBuffer[i]);
            try {
                theInteger = Int32.Parse(sb.ToString());
            }
            catch (FormatException fe) {
                throw new IntegerException("Invalid Integer buffer format. " + fe.Message);
            }
            catch (OverflowException oe) {
                throw new IntegerException("Invalid Integer buffer format. " + oe.Message);
            }
        }
        #endregion

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            byte[] beEncode = this.BeEncode();
            for (int i = 0; i < beEncode.Length; i++)
                sb.Append((char) beEncode[i]);
            return sb.ToString();
        }

        #region Properties
        public int IntegerValue {
            get { return theInteger; }
        }

        #endregion

        #region IBeType Members

        public void Set(int value) {
            theInteger = value;
        }

        public void Set(string value) {
            try {
                theInteger = Int32.Parse(value);
            }
            catch (FormatException) {
                throw new IntegerException("Can convert (" + value + ") to a integer string.");
            }
        }

        public byte [] BeEncode() {
            string integerToStr = theInteger.ToString();
            beEncoded = new byte[integerToStr.Length+2];
            beEncoded[0] = (byte) 'i';
            for (int i = 1; i <= integerToStr.Length; i++)
                beEncoded[i] = (byte) integerToStr[i-1];
            beEncoded[integerToStr.Length + 1] = (byte)'e';
            return beEncoded;
        }

        #endregion
    }
}
