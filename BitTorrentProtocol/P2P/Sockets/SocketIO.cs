#region Using directives

using System;
using System.Net.Sockets;
using SharpTorrent.BitTorrentProtocol.Exceptions;

#endregion

namespace SharpTorrent.BitTorrentProtocol.P2P.Sockets {
    
    public delegate void MessageHandler(SocketIO socket, int numberOfBytes);
    public delegate void CloseHandler(SocketIO socket);
    public delegate void ErrorHandler(SocketIO socket, Exception error);
    /// <summary>
    /// This class supports all Asynchronous sockets.
    /// </summary>
    public class SocketIO {
        /// <summary>
        /// 512 Kb for the ReceiveBuffer
        /// </summary>
        private const int TCPCLIENTBUFFERSIZE = 524288;
        private NetworkStream ns;
        private TcpClient tcpClient;
        private MessageHandler messageHandler;
        private CloseHandler closeHandler;
        private ErrorHandler errorHandler;
        private Boolean disposed;
        private string ip;
        private int port;
        private byte[] receiveBuffer;
        private int bufferSize;
        private int listerIndex;

        #region Construtor and Destructor

        public SocketIO(int bufferSize, MessageHandler mh, CloseHandler ch, ErrorHandler eh) {
            ns = null;
            tcpClient = null;
            messageHandler = mh;
            closeHandler = ch;
            errorHandler = eh;
            disposed = false;
            this.bufferSize = bufferSize;
            receiveBuffer = new Byte[this.bufferSize];
            listerIndex = -1;
        }

        /*public SocketIO(int bufferSize, MessageHandler mh, CloseHandler ch, ErrorHandler eh, SocketIO socket, int listenerIndex) {
            
        }*/

        public ~SocketIO() {
            if (!disposed)
                Dispose();
        }

        #endregion

        #region Private Methods

        private Boolean IsAListenerCreatedSocket() {
            return (listerIndex >= 0);
        }

        private void Dispose() {
            disposed = true;
            Disconnect();
        }

        private void Receive() {
            // If the NetworkStream is valid and we can Read information
            if ((ns != null) && (ns.CanRead)) {
                ns.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(ReceiveComplete), null);
            }
            else
                throw new SocketIOException("Socked closed.");
        }

        private void ReceiveComplete(IAsyncResult ar) {
            // We have a complete message from the other side.
            if ((ns != null) && (ns.CanRead)) {
                int bReceived = ns.EndRead(ar);
                if (bReceived > 0) {
                    messageHandler(this, bReceived);
                }
                Receive();
            }
            else {
                throw new SocketIOException("Connection closed.");
                closeHandler(this);
                Dispose();
            }
        }

        private void SendComplete(IAsyncResult ar) {
            if ((ns != null) && (ns.CanWrite)) {
                ns.EndWrite(ar);
            }
        }   

        #endregion

        # region Public Methods

        public void Connect(string ip, int port) {
            this.ip = ip;
            this.port = port;
            // Connect to the socket
            try {
                if (ns == null) {
                    tcpClient = new TcpClient(this.ip, this.port);
                    ns = tcpClient.GetStream();
                    tcpClient.ReceiveBufferSize = TCPCLIENTBUFFERSIZE;
                    tcpClient.SendBufferSize = TCPCLIENTBUFFERSIZE;
                    tcpClient.NoDelay = true;
                    tcpClient.LingerState = new LingerOption(false, 0);
                    Receive();
                }
            }
            catch (SocketException se) {
                throw new SocketIOException("Error while connecting. [" + se.Message + "]", se);
            }
        }

        public void Disconnect() {
            if (ns != null)
                ns.Close();
            if (tcpClient != null)
                tcpClient.Close();
            ns = null;
            tcpClient = null;
        }

        public void Send(byte[] buffer) {
            // If we can write to the socket do it!.
            if ((ns != null) && (ns.CanWrite)) {
                ns.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(SendComplete), null);
            }
            else
                throw new SocketIOException("Socked Closed.");
        }

        #endregion
    }
}
