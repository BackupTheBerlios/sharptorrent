using System;
using System.Collections;
using System.Text;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	public class ListException : Exception {
		public ListException() : base() {
		}
		public ListException(string message) : base(message) {
		}
		public ListException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	
	/// <summary>
	/// Represents a BitTorrent List.
    ///   Lists are encoded as an 'l' followed by their elements (also bencoded) 
    ///   followed by an 'e'. 
    ///   For example l4:spam4:eggse  corresponds to ['spam', 'eggs'].
    /// </summary>
    public class List : BeType, IBeType {
        protected ArrayList elements;

        #region Constructors
        
        public List() :this(0) {
		}

		public List(int numElements) {
			if (numElements > 0)
				elements = new ArrayList(numElements);
			else
				elements = new ArrayList();
        }

        public List(BeEncode.List list) {
            foreach (BeType element in list.elements)
                this.elements.Add(element);
        }

        public List(byte[] buffer) :this(buffer, 0,0) {
        }

        public List(byte[] theBuffer, int pos, int length) {
            // Check the buffer
            if (theBuffer[pos] != (byte)'l')
                throw new ListException("Invalid List buffer format.");
            int index = pos + 1;
            while ((index <= length) && (theBuffer[index] != (byte)'e')) {
                // An Integer
                if (theBuffer[index] == (byte)'i') {
                    // Parse Integer
                    int last = index + 1;
                    while ((last <= length) && (theBuffer[last] != (byte)'e'))
                        last++;
                    BeEncode.Integer element = new BeEncode.Integer(theBuffer, index, last);
                    elements.Add(element);
                    // After 'e'
                    index = last + 1;
                    continue;
                }
                // A list
                if (theBuffer[index] == (byte)'l') {

                }
                // A Dictionary
                if (theBuffer[index] == (byte)'d') {

                }
                // It must be a String

                throw new ListException("Invalid List buffer format.");
            }
        }
        #endregion

        public void Add(BeType element) {
            elements.Add(element);
        }

        #region Properties

        public BeType this[int index] {
            get {
                if ((index >= 0) && (index < elements.Count))
                    return (BeType) elements[index];
                else
                    throw new ListException("Index out of bounds.");
            }
        }

        #endregion

        #region IBeType Members

        public void Set(int value) {
            elements.Add(new BeEncode.Integer(value));
        }

        public void Set(string value) {
            elements.Add(new BeEncode.String(value));
        }
        
        public byte[] BeEncode() {
            StringBuilder sb = new StringBuilder();
            sb.Append('l');
            // A Integer, a String, a List or a Dictionary
            foreach (BeType element in elements) {
                sb.Append(element.ToString());
            }
            sb.Append('e');
            // String to byte Array
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

#endregion
    }
}
