using System;
using System.IO;
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
            this.elements = (Hashtable)dictionary.elements.Clone();
        }

        public Dictionary(byte[] buffer) : this(buffer, 0,0) {
            // Call another Constructor
        }

        public Dictionary(byte[] buffer, int pos, int length) {
            BeParser parser = new BeParser();
            try {
                Dictionary tempDictionary = parser.Parse(buffer);
                // Copy all the elements to this Dictionary
                this.elements = (Hashtable)tempDictionary.elements.Clone();
                // Free memory
                tempDictionary = null;
            }
            catch (BePaserException bpe) {
                /// TODO log error
                throw new DictionaryException("Error parsing the buffer.", bpe);
            }
        }

        #endregion

        #region Public methods

        public void Add(string key, BeType element) {
            elements.Add(key, element);
        }

        public void Add(string key, byte[] element) {
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

        public bool ContainsKey(string key) {
            return (elements.ContainsKey(key));
        }

        public byte[] SpecialValue(string key) {
            if (ContainsKey(key))
                return (byte []) elements[key];
            else
                throw new DictionaryException("Key (" + key + ") not in Dictionary");
        }

        #endregion

        #region IBeType Members

        public void Set(int value) {
            elements.Add(new BeEncode.Integer(value), new BeEncode.Integer(value));
        }

        public void Set(string value) {
            elements.Add(new BeEncode.String(value), new BeEncode.String(value));
        }

        public byte[] BeEncode() {
            MemoryStream mem = new MemoryStream();
            StreamWriter sw = new StreamWriter(mem);
            sw.Write((byte)'d');
            string key;
            BeType value;
            byte[] beencodedKey;
            byte[] beencodedValue;
            IEnumerator keys = elements.Keys.GetEnumerator();
            while (keys.MoveNext()) {
                key = (string) keys.Current;
                beencodedKey = ((IBeType)(new String(key))).BeEncode();
                for (int i = 0; i < beencodedKey.Length; i++)
                    sw.Write(beencodedKey[i]);
                value = (BeType)elements[key];
                beencodedValue = ((IBeType)value).BeEncode();
                for (int i = 0; i < beencodedValue.Length; i++)
                    sw.Write((byte) beencodedValue[i]);
            }
            sw.Write((byte)'e');
            //return Encoding.ASCII.GetBytes(sb.ToString());
            sw.Close();
            return mem.GetBuffer();
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
