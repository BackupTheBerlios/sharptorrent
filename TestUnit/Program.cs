#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SharpTorrent.BitTorrentProtocol;
using SharpTorrent.BitTorrentProtocol.BeEncode;
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

        public void ReadMetainfoFile() {
            BeParser parser = new BeParser();
            Dictionary dict = parser.Parse(@"C:\Proyectos\SharpTorrent\TestUnit\star.trek.enterprise.415.hdtv-lol.[BT].torrent");
            Console.WriteLine(dict.ToString());
            Console.ReadLine();
        }

        static void Main(string[] args) {
            Program test = new Program();
            test.ReadMetainfoFile();
        }
    }
}
