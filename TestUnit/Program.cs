#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SharpTorrent.BitTorrentProtocol;
using SharpTorrent.BitTorrentProtocol.BeEncode;
#endregion

namespace TestUnit {
    class Program {

        static void Main(string[] args) {
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
    }
}
