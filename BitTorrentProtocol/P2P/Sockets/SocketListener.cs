#region Using directives

using System;
using System.Net.Sockets;
using System.Threading;
using SharpTorrent.BitTorrentProtocol.Exceptions;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Sockets {
    
    public delegate void AcceptHandler(SocketIO socket);
    /// <summary>
    /// This class has a Thread to Listen for new connections.
    /// </summary>
    public class SocketListener {
        private TcpListener listener;
        private Thread accept;
        //SocketIO[] clients;
        private int maxClients;
        private int bufferSize;
        private MessageHandler messageHandler;
        private CloseHandler closeHandler;
        private ErrorHandler errorHandler;
        private AcceptHandler acceptHandler;
        private Object criticalSection;
        private Boolean disposed;
        private string listenerIp;
        private int listenerPort;

        #region Constructor

        public SocketListener() {
        }

        #endregion

        #region Private Methods

        private int NextClientPosition() {
        }

        private void Dispose() {

        }

        private void AcceptThread() {

        }

        #endregion


        #region Public Methods

        public void Stop() {

        }



        #endregion

    }
}
