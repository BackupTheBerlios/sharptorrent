using System;
using System.Collections;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Exceptions;

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
	/// <summary>
	/// Represents a BitTorrent dictionarie.
    ///    Dictionaries are encoded as a 'd' followed by a list of alternating keys
    ///    and their corresponding values followed by an 'e'. For example, 
    ///    d3:cow3:moo4:spam4:eggse  corresponds to {'cow': 'moo', 'spam': 'eggs'} 
    ///    and d4:spaml1:a1:bee corresponds to {'spam': ['a', 'b']} . 
    ///    Keys must be strings and appear in sorted order (sorted as raw strings, 
    ///    not alphanumerics).
    /// </summary>
	public class Dictionary : BeType, IBeType {
		protected Hashtable elements;

        #region Constructors
        
        public Dictionary() : this (0) {
		}

        public Dictionary(int numElements) {
            if (numElements > 0)
                elements = new Hashtable(numElements);
            else
                elements = new Hashtable();
        }

        public Dictionary(BeEncode.Dictionary dictionary) {
            
        }

        public Dictionary(byte[] buffer) : this(buffer, 0,0) {
        }

        public Dictionary(byte[] buffer, int pos, int length) {

        }
        #endregion

        public void Add(string key, BeType element) {
            elements.Add(key, element);
        }

        /*public void Add(BeEncode.String key, BeType element) {
            elements.Add(key, element);
        }*/

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            byte[] beEncode = this.BeEncode();
            for (int i = 0; i < beEncode.Length; i++)
                sb.Append((char)beEncode[i]);
            return sb.ToString();
        }

        #region IBeType Members

        public void Set(int value) {
            elements.Add(new BeEncode.Integer(value), new BeEncode.Integer(value));
        }

        public void Set(string value) {
            elements.Add(new BeEncode.String(value), new BeEncode.String(value));
        }

        public byte[] BeEncode() {
            StringBuilder sb = new StringBuilder();
            sb.Append('d');
            string key;
            BeType value;
            IEnumerator keys = elements.Keys.GetEnumerator();
            while (keys.MoveNext()) {
                key = (string) keys.Current;
                value = (BeType)elements[key];
                sb.Append(key.ToString());
                sb.Append(value.ToString());
            }
            sb.Append('e');
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

#endregion

        #region Properties

        public BeType this[string key] {
            get {
                if (!elements.ContainsKey(key))
                    throw new DictionaryException("Key (" + key + ") not in Dictionary");
                // The element is there
                return (BeType)elements[key];
            }
        }

        #endregion

    }
}
