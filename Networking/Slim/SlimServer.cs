using CommonLib.Events;
using CommonLib.GamePong;
using CommonLib.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommonLib.Slim {

    /// <summary>
    /// This is a very simple server, with support for a single client
    /// </summary>
    public class SlimServer {

        // signatures for the events
        public delegate void NewConnectionHandler(Object sender, NewConnectionEventArgs args);
        public delegate void DataReceivedHandler(Object sender, DataReceivedEventArgs args);

        /// <summary>
        /// Event raised when a new connection with a client is stablished
        /// </summary>
        public event NewConnectionHandler NewConnection;

        /// <summary>
        /// Event raised when data is received from a client
        /// </summary>
        public event DataReceivedHandler DataReceived;


        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private StateObject remote; // represents the client

        private Socket server; // socket that listen for incoming connections

        /// <summary>
        /// The socket connected to the remote end-point
        /// </summary>
        public Socket RemoteSocket => remote?.socket;

        public SlimServer() {
        }

        public void StartServer(int port) {

            // Establish the local endpoint for the socket.
            // The DNS name of the computer 
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = ipHostInfo.AddressList[0];

            EndPoint localEndPoint = new IPEndPoint(ip, port);

            // Create a listener for incoming connections
            Log.WriteLine("Starting server...");

            try {

                // create a socket using a stream object, through TCP
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // bind the server to a local end point network node
                server.Bind(new IPEndPoint(IPAddress.Any, port)); // 'Any' indicates all NICs
                server.Listen(1); // backlog = 1, up to 1 client in the pending queue    

                // start listening async (begin/end async pattern)
                server.BeginAccept(new AsyncCallback(AcceptCallback), null);

                Log.WriteLine($"Listening for new connection on all interfaces, at port {port}");

            } catch (Exception e) {
                Log.WriteLine(e.ToString());
            }
        }

        public void StopServer() {

            // disconnect the connected client
            if (remote != null) {
                remote.socket.Shutdown(SocketShutdown.Both);
                remote.socket.Close();
            }
            
            // stop the incoming connection listener
            if (server != null) {
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }

        }
        
        public void Send(string data) {

            var bytes = Encoding.Unicode.GetBytes(data + Constants.EndOfTransmission);

            // Begin sending the data to the remote device. 
            remote.socket.BeginSend(bytes, 0, bytes.Length, 0,
                new AsyncCallback(SendCallback), remote.socket);
        }

        private void SendCallback(IAsyncResult ar) {
            try {
                // Retrieve the socket from the state object.  
                Socket socket = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = socket.EndSend(ar);
                
                Log.WriteLine($"Sent {bytesSent} bytes to client.");

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        // this callback is invoked whenever a new connection is accepted
        private void AcceptCallback(IAsyncResult ar) {

            // terminate the async request (begin/end pattern)
            Socket socket = server.EndAccept(ar);
                        
            Log.WriteLine($"Accepted connection from {socket.RemoteEndPoint}");

            // store the returned socket that now is connected to the client
            remote = new StateObject() { socket = socket };

            // start receiving data for the socket
            socket.BeginReceive(remote.buffer, 0, remote.buffer.Length, SocketFlags.None,
                new AsyncCallback(ReceiveCallback), remote);

            // start listening the next connection request (begin/end pattern)
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);

            // raise the event to inform listeners that we have a new connection
            NewConnection?.Invoke(this, new NewConnectionEventArgs(remote.socket));

        }

        private void ReceiveCallback(IAsyncResult ar) {

            // retrieve the state object that we passed when we began the call
            StateObject state = (StateObject)ar.AsyncState;

            // end the async call, returning the number of bytes received
            int bytesRead = state.socket.EndReceive(ar);

            if (bytesRead > 0) {

                // store the data received so far.
                state.rx.Append(Encoding.Unicode.GetString(state.buffer, 0, bytesRead));

                // check for end of transmission
                var content = state.rx.ToString(); // this is not efficient... but thats not the point
                if (content.IndexOf(Constants.EndOfTransmission) > -1) {
                    // remove delimiter
                    content = content.Replace(Constants.EndOfTransmission, string.Empty);
                    if (!string.IsNullOrWhiteSpace(content)) {
                        // raise an event to inform all listeners
                        DataReceived?.Invoke(this, new DataReceivedEventArgs(content));

                        // clear reception buffers
                        state.rx.Clear();
                    }
                }
            }

            // start receiving next data packet for the socket
            state.socket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None,
                new AsyncCallback(ReceiveCallback), state);

        }
    }
}
