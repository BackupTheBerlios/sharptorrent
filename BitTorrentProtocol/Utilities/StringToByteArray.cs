#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.Utilities {
    public static class StringToByteArray {
        public static byte[] ConvertStringToByteArray(string sourceString) {
            /*byte[] buffer = new byte[sourceString.Length];
            for (int i = 0; i < sourceString.Length; i++)
                buffer[i] = (byte) sourceString[i];
            return buffer;*/
            return System.Text.Encoding.ASCII.GetBytes(sourceString);
        }

        public static string ConvertByteArrayToString(byte[] byteArray) {
            return System.Text.ASCIIEncoding.ASCII.GetString(byteArray);
        }
    }
}
