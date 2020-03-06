using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Server {

    /// <summary>
    /// Arguments when a new data packet is received.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs {

        /// <summary>
        /// The received data (see remarks for reasons of being <see cref="IEnumerable{byte}"/>)
        /// </summary>
        /// <remarks>
        /// The data is represented as an IEnumerable, since converting 
        /// to an array would be quite expensive. Therefore we can avoid that conversion 
        /// if the consumer can work directly with <see cref="IEnumerable{byte}"/> types.
        /// </remarks>
        public IEnumerable<byte> Data { get; }

        public DataReceivedEventArgs(IEnumerable<byte> data) {
            Data = data;
        }        
    }
}
