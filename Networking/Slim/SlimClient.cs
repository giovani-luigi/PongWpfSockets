using CommonLib.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonLib.Slim {

    /// <summary>
    /// This is a very simple client to use with <see cref="SlimServer" />
    /// </summary>
    public class SlimClient {

        public delegate void DataReceivedEventHandler(object sender, Events.DataReceivedEventArgs e);

        public event DataReceivedEventHandler DataReceived;

        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        
        private StateObject state;

        internal IPAddress IpAddress { get; private set; }

        internal EndPoint RemoteEndPoint { get; private set;}

        /// <summary>
        /// Returns the underlying socket used for communication with the server
        /// </summary>
        internal Socket RemoteSocket => state?.socket;

        internal SlimClient() {
        }

        internal void Connect(IPAddress ip, int port) {
            
            IpAddress = ip;
            RemoteEndPoint = new IPEndPoint(ip, port);

            // Create a TCP/IP socket.  
            Socket client = new Socket(IpAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            client.BeginConnect(RemoteEndPoint,
                new AsyncCallback(ConnectCallback), client);
            
            // wait until connection is stablished
            connectDone.WaitOne();

            // save socket connection to the state object
            state = new StateObject() {
                socket = client 
            };

            // starts continuous listening
            StartListening(state);
        }

        internal void Disconnect() {
            
            // check if anyone connected
            if (state == null) return;

            // release the socket
            state.socket.Shutdown(SocketShutdown.Both);
            state.socket.Close();

            state = null;
        }

        private void StartListening(StateObject remote) {

            // Begin receiving the data from the remote device.  
            remote.socket.BeginReceive(remote.buffer, 0, remote.buffer.Length, 0,
                new AsyncCallback(ReceiveCallback), remote);
        }

        private void ConnectCallback(IAsyncResult ar) {
            try {
                
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Log.WriteLine($"Socket connected to {client.RemoteEndPoint.ToString()}");

                // Signal that the connection has been made.  
                connectDone.Set();

            } catch (Exception e) {
                Log.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            try {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.socket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0) {
                    // store the data received so far.
                    state.rx.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                    // check for end of transmission
                    var content = state.rx.ToString(); // this is not efficient... but thats not the point
                    var indexEOF = content.IndexOf(Constants.EndOfTransmission);
                    if (indexEOF > -1) {

                        // capture and erase grab all text up until delimiter
                        content = content.Substring(0, indexEOF);

                        // remove the captured text from string builder
                        state.rx.Remove(0, indexEOF + Constants.EndOfTransmission.Length);

                        // Signal that all bytes have been received
                        DataReceived?.Invoke(this, new Events.DataReceivedEventArgs(content));                        
                    }
                }

                // keep listening for more data
                client.BeginReceive(state.buffer, 0, state.buffer.Length, 0,
                    new AsyncCallback(ReceiveCallback), state);

            } catch (Exception e) {
                Log.WriteLine(e.ToString());
            }
        }

        internal void Send(string data) {
            if (state == null) return;
            var bytes = Encoding.Unicode.GetBytes(data + Constants.EndOfTransmission);
            state.socket?.SendAsync( bytes, SocketFlags.None);
        }


    }
}
