using System;
using System.IO;
using System.Collections;
using System.Text;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.Types {
	/// <summary>
	/// BitTorrent integer type.
	/// </summary>
	public class Integer : IEncode  {
		private Int32 theInteger;

		public Integer() {
		}
		public Integer(Int32 theInteger) {
			this.theInteger = theInteger;
		}
		
		#region Propiedades
		
		public int ToInt {
			get { return theInteger; }
		}
	
		#endregion
		#region IEncode Members
	
		/// <summary>
		/// Integers are represented by an 'i' followed by the number in base 10 
		/// followed by an 'e'. 
		/// 
		/// For example i3e corresponds to 3 and i-3e corresponds to -3. 
		/// Integers have no size limitation. i-0e is invalid. 
		/// All encodings with a leading zero, such as i03e, are invalid, other than i0e, 
		/// which of course corresponds to 0.
		/// </summary>
		/// <returns>BeEncoded Integer</returns>
		public string Encode() {
			return "i" + theInteger.ToString() +  "e";
		}

		#endregion
		#region BeDeEncode
		
		/// <summary>
		/// Integers are represented by an 'i' followed by the number in base 10 
		/// followed by an 'e'. 
		/// </summary>
		/// <param name="toParse">Elements to parse</param>
		/// <returns>The De-Benconding Integer type</returns>
		public static Types.Integer Decode(BeParser toParse) {
			ArrayList digits;
			// Is it an Integer
			if (toParse.NextToken == 'i') {
				// Read the "i"
				toParse.Next();
				// Read until we find the "e" byte.
				digits = new ArrayList();
				do {
					digits.Add(toParse.Next());
				} while ( (toParse.ThereIsNextByte) && (toParse.NextToken != 'e'));
				// Remove the "e" byte from the parser
				toParse.Next();
				byte [] bDigits = new byte [digits.Count];
				Int32 ind = 0;
				foreach (byte d in digits)
					bDigits[ind++] = d;
				ASCIIEncoding asc = new ASCIIEncoding();
				return new Types.Integer(Int32.Parse(asc.GetString(bDigits)));
			}
			else
				throw new Exception("There is not an Integer to retrieve.");
		}

		#endregion
	}
}
