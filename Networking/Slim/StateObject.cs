using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CommonLib.Slim {

    /// <summary>
    /// State object for receiving data from remote device.  
    /// </summary>
    public class StateObject {
        public Socket socket = null;
        private const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE]; // static buffer (stack memory)
        public StringBuilder rx = new StringBuilder();
    }

}
