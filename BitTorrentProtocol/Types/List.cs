using System;
using System.Collections;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Types {
	public class ListException : Exception {
		public ListException() : base() {
		}
		public ListException(string message) : base(message) {
		}
		public ListException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	
	/// <summary>
	/// Represents a BitTorrent List
	/// </summary>
	public class List : IEncode {
		private ArrayList elements;

		public List() :this(0) {
		}

		public List(Int32 numElements) {
			if (numElements > 0)
				elements = new ArrayList(numElements);
			else
				elements = new ArrayList();
		}

		public void Add(object element) {
			elements.Add(element);
		}

		public IEnumerator GetEnumerator() {
			return elements.GetEnumerator();
		}

		#region Propiedades

		#endregion
		#region IEncode Members

		/// <summary>
		/// Lists are encoded as an 'l' followed by their elements (also bencoded) followed
		/// by an 'e'. 
		/// 
		/// For example l4:spam4:eggse corresponds to ['spam', 'eggs']. 
		/// </summary>
		/// <returns>BeEncode List</returns>
		public string Encode() {
			StringWriter sw = new StringWriter();
			// begining
			sw.Write("l");
			// elements
			
			// end
			sw.Write("e");
			sw.Close();
			return sw.ToString();			
		}

		#endregion
		#region BeDeEncode

		/// <summary>
		/// Lists are encoded as an 'l' followed by their elements (also bencoded) followed
		/// by an 'e'.
		/// 
		/// The elements can be a Dictionary, a List, a String or an Integer.
		/// </summary>
		/// <param name="toParse">Elements to parse</param>
		/// <returns>The De-Benconding List type</returns>
		public static Types.List Decode(BeParser toParse) {
			Types.List theList = new Types.List();
			object val = null;

			if (toParse.NextToken == 'l') {
				// Remove the 'l'
				toParse.Next();
				while ( (toParse.ThereIsNextByte) && (toParse.NextToken != 'e') ) {
					switch (toParse.NextToken) {
						case ((byte) 'i'):
							val = Types.Integer.Decode(toParse);
							break;
						case ((byte) 'd'):
							val = Types.Dictionary.Decode(toParse);
							break;
						case ((byte) 'l'):
							val = Types.List.Decode(toParse);
							break;
						default:  // It is a string
							val = Types.String.Decode(toParse);
							break;
					}
					theList.Add(val);
				}
				// Remove the 'e'
				toParse.Next();
				return theList;
			}
			else
				throw new ListException("There is no List to retrieve.");
		}

		#endregion
	}
}
