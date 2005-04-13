#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpTorrent.BitTorrentProtocol.Exceptions;
#endregion

namespace SharpTorrent.BitTorrentProtocol.BeEncode {

    public class BeParser {
        private int actualTokenPos = 0;
        
        public BeParser() {
        }

        private BeEncode.String ParseString(byte [] buffer) {
            String pString = new String();
            // Fisrt the size of the string
            StringBuilder stringSize = new StringBuilder();
            while (buffer[actualTokenPos] != (char)':')
                stringSize.Append((char)buffer[actualTokenPos++]);
            // Remove ':'
            actualTokenPos++;
            int stringLenght = 0;
            try {
                stringLenght = Int32.Parse(stringSize.ToString());
            }
            catch (FormatException) {
                throw new BePaserException("There is not a valid string size in the String at position " + actualTokenPos.ToString());
            }
            StringBuilder tempString = new StringBuilder(stringLenght);
            for (int i = 0; i < stringLenght; i++)
                tempString.Append((char)buffer[actualTokenPos++]);
            // Assign the String
            pString.Set(tempString.ToString());
            return pString;
        }

        private BeEncode.Integer ParseInteger(byte[] buffer) {
            Integer pInteger = new Integer();
            if ((buffer[actualTokenPos] != (char) 'i') || (buffer[actualTokenPos + 1] == (char) 'e'))
                throw new BePaserException("Not a valid interger at position " + actualTokenPos.ToString());
            // Go to next token 
            actualTokenPos++;
            // The integer is a string
            StringBuilder tempInteger = new StringBuilder();
            while (buffer[actualTokenPos] != (char)'e') {
                tempInteger.Append((char)buffer[actualTokenPos++]);
            }
            // There must be a 'e'
            if (buffer[actualTokenPos] != (char)'e')
                throw new BePaserException("There is no 'e' at position " + actualTokenPos.ToString());
            // Remove the 'e'
            actualTokenPos++;
            // Return the new Integer
            try {
                pInteger.Set(tempInteger.ToString());
                return pInteger;
            }
            catch (IntegerException) {
                throw new BePaserException(tempInteger.ToString() + " is not a valid integer value.");
            }
        }

        private BeEncode.List ParseList(byte [] buffer) {
            List pList = new List();
            // Check the List
            if ((buffer[actualTokenPos] != (char)'l') || (buffer[actualTokenPos + 1] == (char)'e'))
                throw new BePaserException("There is not a valid List at position " + actualTokenPos.ToString());
            // Remove the 'l'
            actualTokenPos++;
            // In the List we can found a Integer, a String, a List or a Dictionary.
            while (buffer[actualTokenPos] != (char)'e') {
                switch (buffer[actualTokenPos]) {
                    case (byte) 'd': pList.Add(ParseDictionary(buffer));
                        break;
                    case (byte)'l': pList.Add(ParseList(buffer));
                        break;
                    case (byte)'i': pList.Add(ParseInteger(buffer));
                        break;
                    default: pList.Add(ParseString(buffer));
                        break;
                }
            }
            // We have the list, remove the 'e'
            actualTokenPos++;
            return pList;
        }

        private BeEncode.Dictionary ParseDictionary(byte [] buffer) {
            Dictionary pDictionary = new Dictionary();
            string dictionaryKey;
            BeType dictionaryElement;
            int infoBegining = 0;

            if ((buffer[actualTokenPos] != (char)'d') || (buffer[actualTokenPos + 1] == (char)'e'))
                throw new BePaserException("There is not a valid Dictionary at position " + actualTokenPos.ToString());
            // Remove the 'd'
            actualTokenPos++;
            while (buffer[actualTokenPos] != (char)'e') {
                // First the dictionary key
                dictionaryKey = ParseString(buffer).StringValue;
                // The info Key has an special treat
                if (dictionaryKey.CompareTo("info") == 0) {
                   infoBegining = actualTokenPos;
                }
                // Now the dictionary element
                switch (buffer[actualTokenPos]) {
                    case (byte)'d': dictionaryElement = ParseDictionary(buffer);
                        break;
                    case (byte)'l': dictionaryElement = ParseList(buffer);
                        break;
                    case (byte)'i': dictionaryElement = ParseInteger(buffer);
                        break;
                    default: dictionaryElement = ParseString(buffer);
                        break;
                }
                // We have the KEY and the ELEMENT
                pDictionary.Add(dictionaryKey, dictionaryElement);
                if (dictionaryKey.CompareTo("info") == 0) {
                    int end = actualTokenPos;
                    byte[] temp = new byte[end - infoBegining];
                    for (int i = 0; i < end - infoBegining; i++)
                        temp[i] = buffer[infoBegining + i];
                    pDictionary.Add("infoToHash", temp);
                }
            }
            // Remove the 'e'
            actualTokenPos++;
            return pDictionary;
        }

        public BeEncode.Dictionary Parse(byte [] buffer) {
            // This MUST be a Dictionary
            if ((char)buffer[actualTokenPos] != 'd')
                throw new BePaserException("This is not a valid dictionary.");
            // Create de Dictionary
            Dictionary parsedDictionary = ParseDictionary(buffer);
            return parsedDictionary;
        }

        public BeEncode.Dictionary Parse(string fileName) {
            if (!File.Exists(fileName))
                throw new BePaserException("File (" + fileName + ") does not exist.");
            // Load the file
            byte[] buffer = null;
            try {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                // To read the file
                buffer = new byte[fs.Length];
                br.Read(buffer, 0, buffer.Length);
                br.Close();
                fs.Close();
            }
            catch (IOException ioe) {
                throw new BePaserException("Error while opening file (" + fileName + "). Error: " + ioe.Message);
            }
            // Parse buffer
            if (buffer != null) {
                return Parse(buffer);
            }
            else
                throw new BePaserException("The file (" + fileName + " was empty.");
        }
    }
}
