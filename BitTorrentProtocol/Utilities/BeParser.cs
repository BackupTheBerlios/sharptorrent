using System;

namespace SharpTorrent.BitTorrentProtocol.Utilities {
	/// <summary>
	/// BeParserException Exception
	/// </summary>
	public class BeParserException : Exception {
		public BeParserException() : base() {
		}
		public BeParserException(string message) : base(message) {
		}
		public BeParserException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// This is a helper class to DeEncode a BeEncoded file.
	/// </summary>
	public class BeParser {
		/// <summary>
		/// Buffer to Deencode
		/// </summary>
		private byte [] buffer;
		/// <summary>
		/// Holds the buffer position
		/// </summary>
		Int32 actualPos;

		public BeParser(byte [] file) {
			buffer = file;
			actualPos = -1;
		}

		public byte [] Next(Int32 length) {
			byte [] result = new byte [length];
			if (length < (buffer.Length - actualPos)) {
				for(int i = 0; i < length; i++)
					result[i] = buffer[++actualPos];
				return result;
			} 
			else {
				throw new BeParserException("No more tokens to read if the length is " + length.ToString());
			}
				
		}

		public byte Next() {
			if ((actualPos+1) < buffer.Length)
				return buffer[++actualPos];
			else
				throw new BeParserException("No more tokens to parse !!!");
		}

		public byte [] StaticBuffer(Int32 startIndex, Int32 endIndex) {
			if ((startIndex >= 0) && (endIndex < buffer.Length)) {
				byte [] result = new byte[endIndex - startIndex + 1];
				for (int i = 0; i < result.Length; i++)
					result[i] = buffer[startIndex + i];
				return result;
			}
			else
				throw new BeParserException("No tokens under those limits !!!");
		}

		#region Properties

		public byte NextToken {
			get {
				if ((actualPos + 1) < buffer.Length)
					return buffer[actualPos+1];
				else
					throw new BeParserException("No more tokens to parse !!!");
			}
		}

		public bool ThereIsNextByte {
			get { return (actualPos < buffer.Length); }
		}

		public Int32 ActualBufferPos {
			get { return actualPos; }
		}

	#endregion
	}
}
