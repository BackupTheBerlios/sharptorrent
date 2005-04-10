#region Using directives

using System;
using System.Net;
using System.IO;

#endregion

namespace SharpTorrent.BitTorrentProtocol.Tracker {
    /// <summary>
    /// Stores the State of the Request.
    /// </summary>
    public class RequestState {
        const int BUFFER_SIZE = 1024;
        //public StringBuilder requestData;
        public byte[] bufferRead;
        public WebRequest request;
        public WebResponse response;
        public Stream streamResponse;
        
        public RequestState() {
            bufferRead = new byte[BUFFER_SIZE];
            //requestData = new StringBuilder();
            request = null;
            streamResponse = null;
        }
    }
}
