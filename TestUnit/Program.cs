#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SharpTorrent.BitTorrentProtocol;
using SharpTorrent.BitTorrentProtocol.BeEncode;
using SharpTorrent.BitTorrentProtocol.Tracker;
using SharpTorrent.BitTorrentProtocol.P2P;
using SharpTorrent.BitTorrentProtocol.Cryptography;
using SharpTorrent.BitTorrentProtocol.Utilities;
#endregion

namespace TestUnit {
    class Program {

        public void Test1() {
            IBeType tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String();
            Console.WriteLine(tInt.ToString());
            tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String();
            tInt.Set(-13);
            Console.WriteLine(tInt.ToString());
            tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String("-13");
            Console.WriteLine(tInt.ToString());
            tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String("Hola pueblo");
            Console.WriteLine(tInt.ToString());

            Console.ReadLine();
        }

        public Dictionary ReadMetainfoFile() {
            BeParser parser = new BeParser();
            Dictionary dict = parser.Parse(@"C:\Proyectos\SharpTorrent\TestUnit\star.trek.enterprise.415.hdtv-lol.[BT].torrent");
            return dict;
        }

        public void TestTracker() {
            Dictionary info = ReadMetainfoFile();
            string infoHashString = info["info"].ToString();
            byte[] infoHash = SHA1.HashValue(StringToByteArray.ConvertStringToByteArray(info["info"].ToString()));
            Tracker tracker = new Tracker(new PeerID(), info["announce"].ToString(), infoHash, "83.40.75.65", 6881);
        }

        static void Main(string[] args) {
            Program test = new Program();
            test.TestTracker();
        }
    }
}
