#region Using directives

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SharpTorrent.BitTorrentProtocol.Exceptions;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Sockets {
    
    public delegate void AcceptHandler(SocketIO socket);
    /// <summary>
    /// This class has a Thread to Listen for new connections. Once a new connection
    /// is received it creates a SocketIO class for it.
    /// </summary>
    public class SocketListener {
        private TcpListener listener;
        private Thread accept;
        private int maxClients;
        private int currentClients;
        private int bufferSize;
        private MessageHandler messageHandler;
        private CloseHandler closeHandler;
        private ErrorHandler errorHandler;
        private AcceptHandler acceptHandler;
        private Object criticalSection;
        private Boolean disposed;
        private string listenerIp;
        private int listenerPort;

        #region Constructor and Destructor

        public SocketListener(string listenerIP, int listenerPort, MessageHandler mh, CloseHandler ch, ErrorHandler eh, AcceptHandler ah, int bufferSize, int currentClients) {
            this.listenerIp = listenerIP;
            this.listenerPort = listenerPort;
            errorHandler = eh;
            acceptHandler = ah;
            listener = null;
            accept = null;
            disposed = false;
            criticalSection = new Object();
            this.currentClients = currentClients;
            this.bufferSize = bufferSize;
        }

        ~SocketListener() {
            if (!disposed)
                Stop();
        }

        #endregion

        #region Private Methods

        private void Dispose() {
            if (!disposed) {
                if (accept != null)
                    Stop();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                
                acceptHandler = null;
                errorHandler = null;
                messageHandler = null;
                closeHandler = null;

                disposed = true;
            }
        }

        private void AcceptThread() {
            Socket client = null;
            try {
                listener = new TcpListener(IPAddress.Parse(listenerIp), listenerPort);
                listener.Start();
                for (; ; ) {
                    client = listener.AcceptSocket();
                    if (client.Connected) {
                        Monitor.Enter(criticalSection);
                        if (maxClients < currentClients) {
                            currentClients++;
                            SocketIO ioClient = new SocketIO(client, bufferSize, messageHandler, closeHandler, errorHandler, listenerIp, listenerPort);
                            acceptHandler(ioClient);
                        }
                        else {
                            errorHandler(null, new SocketIOException("No more connections allowed."));
                            client.Close();
                        }
                        Monitor.Exit(criticalSection);
                    }
                }
            }
            catch (SocketException se) {
                if (se.ErrorCode == 10004) {
                    errorHandler(null, se);
                    if (client != null)
                        if (client.Connected)
                            client.Close();
                }
            }
            catch (Exception e) {
                errorHandler(null, e);
                if (client != null)
                    if (client.Connected)
                        client.Close();
            }
        }

        #endregion

        #region Public Methods

        public void Stop() {
            if (accept != null) {
                listener.Stop();
                accept.Join();
                accept = null;
            }
        }

        public void Start() {
            // Accept Thread
            ThreadStart tsThread = new ThreadStart(AcceptThread);
            accept = new Thread(tsThread);
            accept.Start();
        }

        #endregion
    }
}
