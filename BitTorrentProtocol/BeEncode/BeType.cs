#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SharpTorrent.BitTorrentProtocol.BeEncode {
    public interface IBeType {
        void Set(int value);
        void Set(string value);
        byte [] BeEncode();
    }

    public abstract class BeType {
        protected byte[] beEncoded;

        public BeType() {
        }
    }
}
