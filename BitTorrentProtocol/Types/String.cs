using System;
using System.IO;
using System.Text;
using System.Collections;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Types {
	/// <summary>
	/// BitTorrent String type.
	/// </summary>
	public class String : IEncode {
		private string theString;

		public String() {
		}
		public String(string theString) {
			this.theString = theString;
		}
		public String(Types.String theString) {
			this.theString = theString.ToString();
		}

		public new int GetHashCode() {
			Debugging.StringToDebug("String: (" + theString.ToString()+ "," + theString.GetHashCode().ToString()+")");
			return theString.GetHashCode();
		}

		public new bool Equals(object obj) {
			return base.Equals (obj);
		}

		#region Properties

		public override string ToString() {
			return theString.ToString();
		}

		#endregion
		#region IEncode Members
		
		/// <summary>
		/// Strings are length-prefixed base ten followed by a colon and the string. 
		/// 
		/// For example 4:spam corresponds to 'spam'. 
		/// </summary>
		/// <returns>BeEncoded String</returns>
		public string Encode() {
			return theString.Length.ToString() + ":" + theString;
		}

		#endregion
		#region BeDeEncode
	
		/// <summary>
		/// Strings are length-prefixed base ten followed by a colon and the string. 
		/// 
		/// For example 4:spam corresponds to 'spam'.  
		/// </summary>
		/// <param name="toParse">Elements to parse</param>
		/// <returns>The De-Benconding String type</returns>
		public static Types.String Decode(BeParser toParse) {
			ArrayList digits = new ArrayList();
			// Is it a String
			if (Comparations.IsNumeric(toParse.NextToken)) {
				// Read the base ten number.
				do {
					// It must be an int
					if (Comparations.IsNumeric(toParse.NextToken)) 
						digits.Add(toParse.Next());
					else
						break;
				} while (toParse.ThereIsNextByte);
				// We have the string length, remove the ":"
				toParse.Next();
				ASCIIEncoding asc = new ASCIIEncoding();
				byte [] bDigits = new byte [digits.Count];
				Int32 ind = 0;
				foreach (byte d in digits)
					bDigits[ind++] = d;
				Int32 stringLength = Int32.Parse(asc.GetString(bDigits));
				byte [] literal = toParse.Next(stringLength);
				return new Types.String(asc.GetString(literal));
			}
			else
				throw new Exception("There is no String to retrieve.");
		}

		#endregion
	}
}
