using CommonLib.Communication;
using CommonLib.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.GamePong {

    /// <summary>
    /// Implements a server for the game that received a Player 
    /// object from one remote client
    /// </summary>
    public class PongGameServer : PongGame {

        public delegate void NewConnectionEventHandler(object sender, NewConnectionEventArgs e);

        public event NewConnectionEventHandler NewConnection;

        Slim.SlimServer<NetworkPacket> server;

        public PongGameServer(World world, Player playerLeft, Player playerRight) 
            : base(world, playerLeft, playerRight) {
            server = new Slim.SlimServer<NetworkPacket>();
            server.DataReceived += Server_DataReceived; // when client moves his pad
            server.NewConnection += OnNewConnectionAccepted;
            PlayerLeft.ObjectMoved += LocalObjectMoved; // when we move our pad (server is always the left pad)
            Ball.ObjectMoved += LocalObjectMoved; // when ball moves (we are moving it)
        }

        private void OnNewConnectionAccepted(object sender, NewConnectionEventArgs args) {
            NewConnection?.Invoke(this, args); // bubble up the event
        }

        private void LocalObjectMoved(object sender, ObjectMovedEventArgs args) {
            // using the data of this game class, containing a collection of World Objects
            // build a network packet to send to the client using a factory object
            server.Send(new NetworkPacketFactory().AddObjects(WorldObjects).Build());
        }

        private void Server_DataReceived(object sender, DataReceivedEventArgs<NetworkPacket> reveived) {
            PlayerRight.MoveTo(reveived.Data.PlayerRight.Position);            
        } 

        public void StartServer(int port) {
            server.StartServer(port);
        }

        public void StopServer() {
            server.StopServer();
        }

    }
}
