using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharpTorrent.BitTorrentProtocol.Types;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	/// <summary>
	/// Supports all communications between peers
	/// </summary>
	
	/// <remarks>
	/// Exceptions
	/// </remarks>
	public class PeerCommunicationsException : Exception {
		public PeerCommunicationsException () : base() {
		}
		public PeerCommunicationsException (string message) : base(message) {
		}
		public PeerCommunicationsException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	// On new message
	public delegate void NewMessage(Message message);
	public delegate void NewPeerConnection(byte [] buffer, Socket incoming);
	public delegate void NewConnect(byte [] buffer);

	public class PeerCommunications {
		/// <summary>
		/// Socket class to communicate with other peers
		/// </summary>
		private Socket socket = null;
		private Socket listener;
		private string listenerIp;
		private int listenerPort;
		private bool listenerMode = false;
		/// <summary>
		/// Send the messages to be processed
		/// </summary>
		private MessageProcessor messageProcessor;
		/// <summary>
		/// This Thread will be asking for data all the time
		/// </summary>
		private Thread dataAvailableThr = null;
		/// <summary>
		/// This represents our handshake
		/// </summary>
		private Handshake handshake;
		private bool handshaked = false;
		/// <summary>
		/// Statics
		/// </summary>
		private Int32 downloaded = 0;
		private Int32 uploaded = 0;
		
		// Events
		public event NewMessage OnNewMessage;
		public event NewPeerConnection OnNewConnection;
		public event NewConnect OnNewConnect;

		/// <summary>
		/// Constructor
		/// </summary>
		public PeerCommunications() {
			messageProcessor = new MessageProcessor();
			messageProcessor.OnNewMessage += new NewMessage(messageProcessor_OnNewMessage);
		}
		public PeerCommunications(Socket incoming) : this() {
			this.socket = incoming;
		}

		/// <summary>
		/// Check if there are data available on the socket
		/// </summary>
		/// <returns>True if there are data available</returns>
		public bool DataAvailable() {
			// Incoming connections
			if (listenerMode) {
				try {
					Socket incomingSocket = listener.Accept();
					NewConnection(incomingSocket);
				}
				catch (SocketException) {
					Debugging.StringToDebug("No incoming connections...");
				}
			}
			return ((socket != null) && (socket.Connected) && (socket.Available > 0));
		}

		public void RecieveData() {
			string thrName = (dataAvailableThr != null) ? dataAvailableThr.GetHashCode().ToString() : "";
			//while (socket.Connected) {
				Debugging.StringToDebug("Mirando si hay datos con el Thread " + thrName + ": ", "Communications");
				if (DataAvailable()) {
					Debugging.StringToDebug("Hay datos disponibles con el Thread " + thrName + ": ", "Communications");
					if (!handshaked) {
						// Get the handshake
						byte [] peerHandshake = new byte [Handshake.HANDSHAKESIZE];
						socket.Receive(peerHandshake, 0, peerHandshake.Length, SocketFlags.None);
						handshaked = true;
						if (OnNewConnect != null)
							OnNewConnect(peerHandshake);
					}
					byte [] buffer = new byte [socket.Available];
					int recieved = 0;
					try {
						recieved = socket.Receive(buffer);
					}
					catch (SocketException se) {
						Debugging.StringToDebug("Error de comunicaciones. (" + se.Message + ")");
					}
					Debugging.BufferToDebug(buffer);
					if (recieved > 0) {
						// Send the message to the processor
						try {
							messageProcessor.AddBuffer(buffer);
						}
						catch (MessageProcessorException mpe) {
							Debugging.StringToDebug(mpe.Message);
						}
					}
				}
				else
					Debugging.StringToDebug("No hay datos con el Thread " + thrName + ": ", "Communications");
			


				// Suspend some seconds
				//Thread.Sleep(3000);
			//}
			//Debugging.StringToDebug("El peer remoto se ha desconectado" + dataAvailableThr.GetHashCode().ToString(), "Thread");
			Debugging.StringToDebug("Fin de comprobacion de datos", "Communications");
		}

		private void NewConnection(Socket incoming) {
			Debugging.StringToDebug("New peer conection " + incoming.RemoteEndPoint.ToString());
			// Get the handshake
			if (incoming.Available > 0) {
				Debugging.StringToDebug("There are " + incoming.Available.ToString() + " bytes to read.");
				byte [] buffer = new byte[Handshake.HANDSHAKESIZE];
				incoming.Receive(buffer, 0, buffer.Length, SocketFlags.None);
				Debugging.BufferToDebug(buffer);
				// Notify
				if (OnNewConnection != null)
					OnNewConnection(buffer, incoming);
			}		
		}
		
		private void messageProcessor_OnNewMessage(Message message) {
			// Statics
			if (message is Messages.Piece)
				downloaded += ((Messages.Piece) message).PieceLength;
			if (OnNewMessage != null)
				OnNewMessage(message);
		}

		public void Disconnect() {
			try {
				// Stop listen if we are in listener mode
				if (listenerMode) {
					listener.Close();
					listenerMode = false;
				}
				if ( (socket != null) && (socket.Connected) ) {
					socket.Shutdown(SocketShutdown.Both);
					socket.Close();
				}
				if (dataAvailableThr != null)
					dataAvailableThr.Join();
			}
			catch (SocketException se) {
				throw new PeerCommunicationsException("Can't close connection to peer with error (" + se.Message + " and code " + se.ErrorCode.ToString() + ")"); 
			}
		}

		public void Connect(Handshake handshake, IPAddress ip) {
			Connect(handshake, ip, 0);
		}

		public void Connect(Handshake handshake, IPAddress ip, int port) {
			try {
				this.handshake = handshake;
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				
				IPEndPoint remoteEndPoint = new IPEndPoint(ip, port);
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

				socket.Bind(localEndPoint);
				Debugging.StringToDebug("LocalEndPoint = " + ((IPEndPoint) socket.LocalEndPoint).Address.ToString() );
				Debugging.StringToDebug("RemoteEndPoint = " + remoteEndPoint.Address.ToString() + " on port " + port.ToString());
				socket.Connect(remoteEndPoint);
				socket.Blocking = false;
				
				// Send handshake
				Send(handshake);

				// Create a thread to ask every time for data available
				dataAvailableThr = new Thread(new ThreadStart(RecieveData));
				Debugging.StringToDebug("Creado Thread connect " + dataAvailableThr.GetHashCode().ToString(), "Thread");
				dataAvailableThr.Start();
			}
			catch (SocketException se) {
				throw new PeerCommunicationsException("Can't create socket. (" + se.Message + " and error code " + se.ErrorCode.ToString() + ")");
			}
		}

		public void Listen() {
			// Listener
			try {
				listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(listenerIp), listenerPort);
				// Bind to port and Accept connections
				listener.Bind(localEndPoint);	
				listener.Blocking = false;
				listener.Listen(1);
				listenerMode = true;
			}
			catch (SocketException se) {
				throw new PeerCommunicationsException("Can't create listener socket. (" + se.Message + " with error code " + se.ErrorCode.ToString() + ")");
			}
		}

		public void Send(Message message) {
			try {
				if (message is Messages.Piece)
					uploaded += ((Messages.Piece) message).PieceLength;
				Debugging.StringToDebug("Enviando mensaje (" + Utilities.HexEncoding.ToString(message.ByteMessage())  + ")" , "SendMessage");	
				Int32 sended = socket.Send(message.ByteMessage(), message.ByteMessage().Length, SocketFlags.None);
				Debugging.StringToDebug("Enviados " + sended.ToString(), "SendMessage");
			}
			catch (SocketException se) {
				throw new PeerCommunicationsException("Can't send data to peer with error (" + se.Message + " and error code " + se.ErrorCode.ToString() + ")");
			}
		}

		#region Properties

		public string ListenerIP {
			set { listenerIp = value; }
		}

		public int ListenerPort {
			set { listenerPort = value; }
		}

		public int Downloaded {
			get { return downloaded; }
		}

		public int Uploaded {
			get { return uploaded; }
		}

		public bool InListenerMode {
			get { return listenerMode; }
		}

		#endregion
	}
}
