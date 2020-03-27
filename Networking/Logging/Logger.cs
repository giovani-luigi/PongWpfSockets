using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CommonLib.Logging {
    public static class Log {

        private static TextWriter _log = null;
        private static TextWriter MyLog {
            get {
                if (_log != null) return _log;
                return Console.Out; // use default
            }
        }

        public static void SetLogOutput(TextWriter logger) {
            _log = logger;
        }

        internal static void WriteLine(string line) {
            MyLog.WriteLine(line);
        }

    }
}
