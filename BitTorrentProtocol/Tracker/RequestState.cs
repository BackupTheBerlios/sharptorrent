#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.Tracker {
    /// <summary>
    /// Stores the State of the Request.
    /// </summary>
    public class RequestState {
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] bufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        
        public RequestState() {
            bufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder();
            request = null;
            streamResponse = null;
        }
    }
}
