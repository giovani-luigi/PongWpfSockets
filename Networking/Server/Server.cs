using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;


namespace CommonLib.Server {

    /// <summary>
    /// Base class that implements a multi client general purpose server using sockets
    /// </summary>
    public class Server {

#region Public properties and fields

        // signatures for the events
        public delegate void NewConnectionHandler(Object sender, NewConnectionEventArgs args);
        public delegate void DataReceivedHandler(Object sender, DataReceivedEventArgs args);

        /// <summary>
        /// Event raised when a new connection with a client is stablished
        /// </summary>
        public event NewConnectionHandler NewConnection;
        public event DataReceivedHandler DataReceived;
        
        /// <summary>
        /// Port to listen for connections and receive/send data
        /// </summary>
        public int Port { get; private set; }

#endregion

        // set of objects regarding each active client connection
        private class ClientConnection {
            public Socket socket = null;
            private const int BUFFER_SIZE = 1024;
            public byte[] buffer = new byte[BUFFER_SIZE]; // static buffer (stack memory)
            public IList<IEnumerable<byte>> data = new List<IEnumerable<byte>>(BUFFER_SIZE);
        }

        private readonly System.IO.TextWriter LogOutput;

        // holds a list of all clients connected
        private List<ClientConnection> clients = new List<ClientConnection>();

        // the socket that accepts connections
        private Socket server = null;

        protected Server(System.IO.TextWriter logOutput) {
            LogOutput = logOutput;
        }
        
        public void StartListening(int port) {

            Port = port;

            LogOutput.WriteLine("Starting server...");

            // create a socket using a stream object, through TCP
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // bind the server to a local end point network node
            server.Bind(new IPEndPoint(IPAddress.Any, Port)); // 'Any' indicates all NICs
            server.Listen(1); // backlog = 1, up to 1 client in the pending queue
            
            // start listening async (begin/end async pattern)
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);

            LogOutput.WriteLine($"Listening for new connection on all interfaces, at port {port}");
        }

        // this callback is invoked whenever a new connection is accepted
        private void AcceptCallback(IAsyncResult ar) {
            
            // terminate the async request (begin/end pattern)
            Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);

            // store the returned socket that now is connected to the client
            ClientConnection connection = new ClientConnection();
            clients.Add(new ClientConnection() { socket = socket });

            // start receiving data for the socket
            socket.BeginReceive(connection.buffer, 0, connection.buffer.Length, SocketFlags.None, 
                new AsyncCallback(ReceiveCallback), connection);

            // start listening the next connection request (begin/end pattern)
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);

            // raise the event to inform listeners that we have a new connection
            NewConnection?.Invoke(this, new NewConnectionEventArgs(socket));
        }

        private void ReceiveCallback(IAsyncResult ar) {
            
            // retrieve the socket that we passed when we began the call
            ClientConnection connection = (ClientConnection)ar.AsyncState;

            // end the async call, returning the number of bytes received
            int read = connection.socket.EndReceive(ar);

            if (read > 0) {
                // copy data from static buffer to heap, so next packet can be received
                connection.data.Add(new List<byte>(connection.buffer)); // move by copy
            } else {
                if (connection.data.Count > 0) {
                    // all data has been read. Concatenate all segments into a single IEnumerable
                    // and raise an event to inform all listeners
                    DataReceived(this,
                        new DataReceivedEventArgs(
                            connection.data.Aggregate((result, item) => result.Concat(item))
                            ));
                }
                // start receiving next data packet for the socket
                connection.socket.BeginReceive(connection.buffer, 0, connection.buffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), connection);
            }
        }

    }
}
