#region Using directives
using System;
using System.IO;
#endregion

namespace SharpTorrent.BitTorrentProtocol.FileIO {

    public abstract class Stream {
        protected FileStream fs;
        protected string fileName = String.Empty;

        public Stream() {
        }

    }
}
