using System;
using System.IO;

namespace CommonLib.Server {

    /// <summary>
    /// This class implements a server that is capable of accepting connections,
    /// and also using 'Game' messaging protocol for sendind/receiving data
    /// </summary>
    public class GameServer : Server {

        /// <summary>
        /// Raised when a new data is received from a client.
        /// </summary>
        public new delegate void DataReceivedHandler(object sender, EventArgs e);

        /// <summary>
        /// Creates a new instance of the game server using a given text writer for log output
        /// </summary>
        protected GameServer(TextWriter logOutput) : base(logOutput) {

        }



    }
}
