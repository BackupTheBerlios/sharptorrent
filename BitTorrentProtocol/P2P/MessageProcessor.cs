using System;
using System.IO;
using System.Net;
//using System.Collections;
using SharpTorrent.BitTorrentProtocol.P2P.Messages;
using SharpTorrent.BitTorrentProtocol.Utilities;

namespace SharpTorrent.BitTorrentProtocol.P2P {
	public class MessageProcessorException : Exception {
		public MessageProcessorException () : base() {
		}
		public MessageProcessorException (string message) : base(message) {
		}
		public MessageProcessorException(string message, Exception innerException) : base(message, innerException) {
		}
	}
	/// <summary>
	/// This class recieves all the messages and process it. 
	/// 
	/// </summary>
	public class MessageProcessor {
		// Message stream
		private MemoryStream stream = new MemoryStream();
		private int messageLength;

		// Events
		public event NewMessage OnNewMessage;

		public MessageProcessor() {
		}

		private void ProcessMessages() {
			Debugging.StringToDebug("Processing message streams");
			
			// Message start with the message length. Message length == 0 means KeepAlive
			/*byte [] lengthArray = new byte[BigEndian.BIGENDIANBYTELENGTH];
			for (int ind = 0; ind < lengthArray.Length; ind++)
				lengthArray[ind] = (byte) stream.ReadByte();
			messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthArray, 0));*/
			messageLength = BigEndian.FromBigEndian(stream);
			
			// Pueden ser solo KeepAlives
			if (messageLength == 0)
				Debugging.MessageToDebug(new KeepAlive());

			// If the message isn't completed
			if (messageLength < 0) {
				// Create a new message and discard KeepAlives
				/// POr hacer
				/*RemoveFromStream();
				*/
				throw new MessageProcessorException();
			}
			else {
				Debugging.StringToDebug("Processing a message");
				// Process the message
				if (OnNewMessage != null)
					OnNewMessage(DoMessage(messageLength));
				Debugging.StringToDebug("Message processed");
			}
			Debugging.StringToDebug("End processing message streams");
		}

		/// <summary>
		/// Construct a new Message.
		/// 
		/// Message types:
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
		/// Example:
		///		hex values:  00 00 00 0D 05 FF FF FF FF FF FF FF FF FF FF FF E0 
		///		byte values: 0 0 0 13 5 255 255 255 255 255 255 255 255 255 255 255 224
		/// </summary>
		/// <param name="buffer">Recieved buffer</param>
		/// <returns>Returns the recieved message</returns>
		private Message DoMessage(int MessageLength) {
			Message returnMessage;
			switch (stream.ReadByte()) {
				case 0:	returnMessage = new Choke();
					break;
				case 1: returnMessage = new UnChoke();
					break;
				case 2: returnMessage = new Interested();
					break;
				case 3:	returnMessage = new UnInterested();
					break;
				case 4:	returnMessage = new Have(stream.ReadByte());
					break;
				case 5: byte [] bitField = new byte [MessageLength];
								for (int i = 0; i < MessageLength; i++)
									bitField[i] = (byte) stream.ReadByte();
          			returnMessage = new Bitfield(bitField);
					break;
				case 6:	returnMessage = new Request(stream); //stream.ReadByte(), stream.ReadByte(), stream.ReadByte());
					break;
				case 7:	returnMessage = new Piece(stream.ReadByte(), stream.ReadByte(), new byte [2]);
					break;
				case 8: returnMessage = new Cancel(stream.ReadByte(), stream.ReadByte(), stream.ReadByte());
					break;
				default:throw new MessageProcessorException("Invalid message type...");
			}
			// Remove the message from Stream
			RemoveFromStream();
			Debugging.MessageToDebug(returnMessage);
			return returnMessage;
		}

		/// <summary>
		/// Remove from 0 to Position.
		/// </summary>
		private void RemoveFromStream() {
			/*int totalSize = (int) (stream.Length - stream.Position);
			if (totalSize > 0) {
				byte [] buffer = new byte[totalSize * 2];
				stream.Read(buffer, (int) stream.Position, (int) stream.Length);
				stream = new MemoryStream(buffer, 0, buffer.Length);
			}
			else // Remove all the stream */
			if (stream.Length == stream.Position)
				stream = new MemoryStream();
			//else
			//	stream = new MemoryStream(stream.GetBuffer(), (int) stream.Position, (int) (stream.Length - stream.Position));
		}

		public void AddBuffer(byte [] buffer) {
			try {
				stream.Write(buffer, 0, buffer.Length);
				stream.Position -= buffer.Length;
			}
			catch (ArgumentNullException ane) {
				throw new MessageProcessorException("Null buffer...");
			}
			try {
				while (stream.Length > 0)
					ProcessMessages();
			}
			catch (MessageProcessorException mpe) {
				if (mpe.Message.Length > 0)
					throw mpe;
			}
		}
	}
}
