using System;
using System.Text;
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
    public class IntegerException : Exception {
		public IntegerException() : base() {
		}
		public IntegerException(string message) : base(message) {
		}
        public IntegerException(string message, Exception innerException) : base(message, innerException) {
        }
    }
    public class Integer : BeType, IBeType {
		private int theInteger;

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
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
                buffer[i] = theBuffer[pos + i];
            // Check the Buffer
            if (buffer[0] != (byte) 'i')
                throw new IntegerException("Invalid Integer buffer format.");
            if ( (buffer[1] == (byte) '0') && (buffer[2] != (byte) 'e'))
                throw new IntegerException("Invalid Integer buffer format. The Integer starts with 0.");
            // Holds the number
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < buffer.Length - 1; i++)
                sb.Append((char)buffer[i]);
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
            theInteger = Int32.Parse(value);
        }

        public byte [] BeEncode() {
            string integerToStr = theInteger.ToString();
            beEncoded = new byte[integerToStr.Length+2];
            beEncoded[0] = (byte) 'i';
            for (int i = 1; i < integerToStr.Length; i++)
                beEncoded[i] = (byte) integerToStr[i-1];
            beEncoded[integerToStr.Length + 1] = (byte)'e';
            return beEncoded;
        }

        #endregion
    }
}
