using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Events {

    /// <summary>
    /// Arguments when a new data packet is received.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs {

        /// <summary>
        /// The received data
        /// </summary>
        /// <remarks>
        public string Data { get; }

        public DataReceivedEventArgs(string data) {
            Data = data;
        }
    }

    /// <summary>
    /// Arguments when a new data packet is received.
    /// </summary>
    public class DataReceivedEventArgs<T> : EventArgs {

        public T Data { get; }

        public DataReceivedEventArgs(T data) {
            Data = data;
        }
    }
}
