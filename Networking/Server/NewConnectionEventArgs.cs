using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommonLib.Server {
    public class NewConnectionEventArgs : EventArgs {
        
        public Socket Socket { get; }

        public NewConnectionEventArgs(Socket socket) {
            Socket = socket;
        }
    }
}
