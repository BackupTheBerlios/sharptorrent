#region Using directives

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
#endregion

namespace SharpTorrent.BitTorrentProtocol.Log {
    public enum LogType { Information, Warning, Exception };
    public class Log : TraceListener {
        private string fileName;
        private StreamWriter sw;

        #region Constructors

        public Log(string fileName) {
            this.fileName = fileName;
            // Prepare the log file
            sw = new StreamWriter(this.fileName);
            this.WriteLine("Starting log", LogType.Information);

        }

        #endregion

        #region Private methods

        private string FormatMessage(string message, LogType type) {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToShortDateString());
            sb.Append(";");
            sb.Append(DateTime.Now.ToShortTimeString());
            sb.Append(";");
            sb.Append(message);
            sb.Append(";");
            sb.Append(type.ToString());
            return sb.ToString();
        }

        private void Write(string message, LogType type) {
            sw.Write(FormatMessage(message, type));
            Flush();
        }

        private void WriteLine(string message, LogType type) {
            sw.WriteLine(FormatMessage(message, type));
            Flush();
        }

        #endregion

        #region Public methods

        public override void Close() {
            sw.Close();
        }

        public override void Flush() {
            base.Flush();
            sw.Flush();
        }

        #endregion

        #region TraceListener stub

        public override void Write(string message) {
            Write(message, LogType.Information);
        }

        public override void WriteLine(string message) {
            WriteLine(message, LogType.Information);
        }

        #endregion
    }
}
