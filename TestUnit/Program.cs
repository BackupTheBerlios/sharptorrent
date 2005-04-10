#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SharpTorrent.BitTorrentProtocol;
using SharpTorrent.BitTorrentProtocol.BeEncode;
using SharpTorrent.BitTorrentProtocol.Tracker;
using SharpTorrent.BitTorrentProtocol.P2P;
using SharpTorrent.BitTorrentProtocol.Utilities;
#endregion

namespace TestUnit {
    class Program {
        Tracker tracker = null;

        public void Test1() {
            /*tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String();
            tInt.Set(-13);
            Console.WriteLine(tInt.ToString());
            tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String("-13");
            Console.WriteLine(tInt.ToString());
            tInt = new SharpTorrent.BitTorrentProtocol.BeEncode.String("Hola pueblo");
            Console.WriteLine(tInt.ToString());*/
            Dictionary dic = new Dictionary();
            List lista = new List(3);
            lista.Add(new SharpTorrent.BitTorrentProtocol.BeEncode.String("Preciosa"));
            lista.Add(new SharpTorrent.BitTorrentProtocol.BeEncode.String("amable"));
            lista.Add(new Integer(10));
            dic.Add("miriam", lista);
            byte[] bee = dic.BeEncode();

            Console.ReadLine();
        }

        public Dictionary ReadMetainfoFile() {
            BeParser parser = new BeParser();
            Dictionary dict = parser.Parse(@"C:\Proyectos\SharpTorrent\TestUnit\absetup.exe.torrent");
            return dict;
        }

        public void TestTracker() {
            Dictionary info = ReadMetainfoFile();
            byte[] infoToHash = info.SpecialValue("infoToHash");
            tracker = new Tracker(new PeerID(), info["announce"].ToString(), infoToHash, "83.40.75.65", 6881);
            tracker.Downloaded = 0;
            tracker.Uploaded = 0;
            tracker.Left = 0;
            tracker.onNewTrackerResponse += new NewTrackerResponse(tracker_onNewTrackerResponse);
            tracker.onNewPeers += new NewPeersEventHandler(tracker_onNewPeers);
            try {
                tracker.StartTrackerRequests();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args) {
            Program test = new Program();
            //test.Test1();
            test.TestTracker();
            Console.ReadLine();
        }

        void tracker_onNewPeers(Tracker tracker) {
            Console.WriteLine("Nuevos Peers desde el tracker");
            Peers peers = tracker.GetPeers();
            // Parar el tracker
            tracker.StopTrackerRequests();
        }

        void tracker_onNewTrackerResponse() {
            Console.WriteLine("Respuesta del tracker.");
            if (tracker.RequestFailure)
                Console.WriteLine("Repuesta fallida. (" + tracker.RequestFailureReason + ")");
        }
    }
}
