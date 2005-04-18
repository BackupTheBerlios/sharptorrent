using System;
using System.Net;
using SharpTorrent.BitTorrentProtocol.Exceptions;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P.Messages {
	/// <summary>
	/// Represents a Peer2Peer message.
	/// 
	/// After Handshake all later integers sent in the protocol are encoded as 
	/// four bytes big-endian.
	/// 
	/// Messages comes as alternating stream of length prefixes and messages. 
	/// Messages of length zero are keepalives, and ignored. 
	/// 
	/// All non-keepalive messages start with a single byte which gives their type. 
	/// The possible values are:
	/// 
	/// 0 - choke 
	/// 1 - unchoke 
	/// 2 - interested 
	/// 3 - not interested 
	/// 4 - have 
	/// 5 - bitfield 
	/// 6 - request 
	/// 7 - piece 
	/// 8 - cancel 
	/// 
	/// 'choke', 'unchoke', 'interested', and 'not interested' have no payload.
	/// </summary>
    public interface IMessage {
        byte [] ToStream();
    }

    public abstract class Message {
        protected int messagePosition;
        protected byte type;
		protected byte [] message;

		public Message() {
            messagePosition = 0;
        }

		public new string ToString() {
            return Conversions.HexByteArrayToString(message);
        }

        protected byte [] ToStream() {
            // The basic types are 5 bytes length
            return new byte[BigEndian.BIGENDIANBYTELENGTH + 1];
        }

        protected void AddMessage(byte[] buffer, byte[] newMessage) {
            // Check for space
            if (buffer.Length < messagePosition + newMessage.Length)
                throw new MessageException("Error adding a message to the buffer. (byte[])");
            for (int i = 0; i < newMessage.Length; i++)
                buffer[messagePosition++] = newMessage[i];
        }

        protected void AddMessage(byte[] buffer, byte value) {
            if (messagePosition > buffer.Length)
                throw new MessageException("Error adding a message to the buffer. (byte)");
            buffer[messagePosition++] = value;
        }
    }
}
