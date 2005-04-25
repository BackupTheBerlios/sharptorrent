#region Using directives
using System;
using System.Diagnostics;
#endregion

namespace SharpTorrent.BitTorrentProtocol.Exceptions {
    
    public class BitTorrentException : ApplicationException {
        public BitTorrentException() : base() {
            Trace.WriteLine("Exception generated - type (" + this.ToString() + ") - message (" + this.Message +")");
        }
        public BitTorrentException(string message) : base(message) {
            Trace.WriteLine("Exception generated - type (" + this.ToString() + ") - message (" + message + ")");
        }
        public BitTorrentException(string message, Exception innerException) : base(message, innerException) {
            Trace.WriteLine("Exception generated - type (" + this.ToString() + ") - message (" + message + ")");
        }
    }

    public class BePaserException : BitTorrentException {
        public BePaserException() : base() {
        }
        public BePaserException(string message) : base(message) {
        }
        public BePaserException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class IntegerException : BitTorrentException {
        public IntegerException() : base() {
        }
        public IntegerException(string message) : base(message) {
        }
        public IntegerException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class StringException : BitTorrentException {
        public StringException() : base() {
        }
        public StringException(string message) : base(message) {
        }
        public StringException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class ListException : BitTorrentException {
        public ListException() : base() {
        }
        public ListException(string message) : base(message) {
        }
        public ListException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class DictionaryException : BitTorrentException {
        public DictionaryException() : base() {
        }
        public DictionaryException(string message) : base(message) {
        }
        public DictionaryException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class TrackerException : BitTorrentException {
        public TrackerException() : base() {
        }
        public TrackerException(string message) : base(message) {
        }
        public TrackerException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class SocketIOException : BitTorrentException {
        public SocketIOException() : base() {
        }
        public SocketIOException(string message) : base(message) {
        }
        public SocketIOException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class SocketListenerException : BitTorrentException {
        public SocketListenerException() : base() {
        }
        public SocketListenerException(string message) : base(message) {
        }
        public SocketListenerException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class MessageException : BitTorrentException {
        public MessageException() : base() {
        }
        public MessageException(string message) : base(message) {
        }
        public MessageException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class StreamOperationsException : BitTorrentException {
        public StreamOperationsException() : base() {
        }
        public StreamOperationsException(string message) : base(message) {
        }
        public StreamOperationsException(string message, Exception innerException) : base(message, innerException) {
        }
    }

    public class FileException : BitTorrentException {
        public FileException() : base() {
        }
        public FileException(string message) : base(message) {
        }
        public FileException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
