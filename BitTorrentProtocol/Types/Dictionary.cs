using System;
using System.Collections;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Types {
	/// <summary>
	/// Dictionary Exception
	/// </summary>
	public class DictionaryException : Exception {
		public DictionaryException() : base() {
		}
		public DictionaryException(string message) : base(message) {
		}
		public DictionaryException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// Represents a BitTorrent dictionarie.
	/// </summary>
	public class Dictionary : IEncode {
		protected Hashtable elements;

		public Dictionary()	:this(0) {
		}

		public Dictionary(Int32 numElements) {
			if (numElements > 0) 
				elements = new Hashtable(numElements);
			else
				elements = new Hashtable();
		}

		public void Add(object keyName, object keyValue) {
			elements.Add(keyName, keyValue);
		}

		public bool ContainsKey(string key) {
			// Key is a string so BeCode
			//Types.String sKey = new Types.String(key);
			return elements.ContainsKey(key);
		}

		#region Properties

		public object this[string key] {
			get {
				if (!ContainsKey(key))
					throw new DictionaryException("Key " + key.ToString() + " doesn't exist in this dictionary.");
				// Contains the element
				//Types.String sKey = new Types.String(key);
				return elements[key];
			}
		}
		
		#endregion
		#region Enumerator

		public IDictionaryEnumerator GetEnumerator() {
			return elements.GetEnumerator();
		}

		#endregion
		#region IEncode Members
		/// <summary>
		/// Dictionaries are encoded as a 'd' followed by a list of alternating keys 
		/// and their corresponding values followed by an 'e'. 
		/// 
		/// For example, d3:cow3:moo4:spam4:eggse corresponds to 
		/// {'cow': 'moo', 'spam': 'eggs'} and 
		/// d4:spaml1:a1:bee corresponds to {'spam': ['a', 'b']} . 
		/// 
		/// <b><i>Keys</i> must be <i>strings</i></b> and appear in sorted order 
		/// (sorted as raw strings, not alphanumerics).
		/// </summary>
		/// <returns>BeEncode Dictionary</returns>
		public string Encode() {
			StringWriter sw = new StringWriter();
			// Begining with a 'd'
			sw.Write("d");
			// Gets a enumrator for each element
			IDictionaryEnumerator ie = elements.GetEnumerator();
			while (ie.MoveNext()) {
				// Key element is a string
				sw.Write( ((Types.String) ie.Key).Encode());
				// Value element can be a string, an integer or a List
				sw.Write( ((IEncode) (ie.Value)).Encode());
				/*Types.String sValue = ie.Value as Types.String;
				if (sValue != null) {
					sw.Write(sValue.Encode());
					continue;
				}
				Types.Integer iValue = ie.Value as Types.Integer;
				if (iValue != null) {
					sw.Write(iValue.Encode());
					continue;
				}
				Types.List lValue = ie.Value as Types.List;
				if (lValue != null) {
					sw.Write(lValue.Encode());
					continue;
				}
				// In this case I don't know what to do !!!
				throw new Exception("Unknown type to BeEncode...");*/
			}
			// Ends with a 'e'.
			sw.Write("e");
			sw.Close();
			return sw.ToString();
		}
		#endregion
		#region BeDeEncode

		/// <summary>
		/// To decode a Dictionary we must read the <i>Key</i> as a <b>string</b>.
		/// The value can be a String, an Integer, a List or another Dictionary. There can
		/// be more values in a Dictionary.
		/// </summary>
		/// <param name="toParse">Elements to parse</param>
		/// <returns>The De-Benconding Dictionary type</returns>
		public static Types.Dictionary Decode(BeParser toParse) {
			Types.Dictionary theDictionary = new Dictionary();
			Int32 infoDS = 0, infoDE;

			object val = null;
			// Is it a dictionay ?
			if (toParse.NextToken == 'd') {
				// Read the "d" byte
				toParse.Next();
				while ( (toParse.ThereIsNextByte) && (toParse.NextToken != 'e') ) {
					// Now we read a String
					//Types.String key = Types.String.Decode(toParse);
					string key = Types.String.Decode(toParse).ToString();
					// Now we can read a String, an Integer, a List or another Dictionary
					switch (toParse.NextToken) {
						case ((byte) 'i'):	
							val = Types.Integer.Decode(toParse);
							break;
						case ((byte) 'l'):
							val = Types.List.Decode(toParse);
							break;
						case ((byte) 'd'):	// This is a Dictionary
							// Need the info buffer to hash with SHA1
							if (key == "info") {
								infoDS = toParse.ActualBufferPos + 1;		// Including the 'd'
							}
							val = Types.Dictionary.Decode(toParse);
							if (key == "info") {
								infoDE = toParse.ActualBufferPos;		// Including the 'e'
								// Need to store the info byte buffer
								byte [] infoBuffer = toParse.StaticBuffer(infoDS, infoDE);
								theDictionary.Add("toSHA1info", infoBuffer);
							}
							break;
						default:
						  val = Types.String.Decode(toParse);
							break;
					}
					theDictionary.Add(key, val);
				}
				// Remove the 'e'
				toParse.Next();
				return theDictionary;
			}
			else
				throw new DictionaryException("There is no Dictionary to retrieve.");
		}

		#endregion
	}
}
