using System;
using System.Net;
using System.IO;

namespace SharpTorrent.BitTorrentProtocol.Utilities {
	/// <summary>
	/// Helper class to convert to and from BigEndian
	/// </summary>
	public class BigEndian {
		public const byte BIGENDIANBYTELENGTH = 4;

		public BigEndian() {
		}

		public static void ToBigEndian(int integerValue, ref byte [] buffer, int initialPosition) {
			byte [] converted = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(integerValue));
			for (int i = 0; i < converted.Length; i++)
				buffer[initialPosition + i] = converted[i];
		}

        public static byte [] ToBigEndian(int integerValue) {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(integerValue));
        }

        /*public static int FromBigEndian(byte [] Buffer, int InitialPos) {
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Buffer, InitialPos));
		}

		public static int FromBigEndian(MemoryStream stream) {
			byte [] buffer = new byte[BigEndian.BIGENDIANBYTELENGTH];
			for (int i = 0; i < buffer.Length;i++)
				buffer[i] = (byte) stream.ReadByte();
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
        }*/		
	}
}
