using System;
using System.IO;

namespace SharpTorrent.BitTorrentProtocol.Types {
	/// <summary>
	/// This represents a Peer id
	/// </summary>
	public class PeerID {
		public const int PEERIDSIZE = 20;
		private byte [] id;

		public PeerID(byte [] id) {
			this.id = id;
		}
		public PeerID(string id) {
			if (id.Length != PEERIDSIZE)
				throw new Exception("Invalid peer id.");
			this.id = new byte [PEERIDSIZE];
			for(int i = 0; i < PEERIDSIZE; i++)
				this.id[i] = (byte) id[i];
		}

		public new string ToString() {
			StringWriter sw = new StringWriter();
			foreach (byte val in id)
				sw.Write((char) val);
			sw.Close();
			return sw.ToString();
		}

		public static bool operator !=(PeerID idLeft, PeerID idRight) {
			// Introducir mas optimizacion
			bool identicals = true;
			for(int i = 0; i < idLeft.id.Length; i++) {
				if (idLeft.id[i] != idRight.id[i]) {
					identicals = false;
					break;
				}
			}
			return !identicals;
		}

		public static bool operator ==(PeerID idLeft, PeerID idRight) {
			// Introducir mas optimizacion
			bool identicals = true;
			for(int i = 0; i < idLeft.id.Length; i++) {
				if (idLeft.id[i] != idRight.id[i]) {
					identicals = false;
					break;
				}
			}
			return identicals;
		}

		public override bool Equals(object obj) {
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return base.GetHashCode ();
		}

		#region Properties

		public byte [] Id {
			get { return id; }
		}

		#endregion
	}
}
