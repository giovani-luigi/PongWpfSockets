using CommonLib.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CommonLib.GamePong {

    /// <summary>
    /// Reprensets a game client which receives game 
    /// updates from a remote end-point
    /// </summary>
    public class PongGameClient : PongGame {

        Slim.SlimClientOfObject<PongGame> client;

        public PongGameClient(World world, Player playerLeft, Player playerRight) 
            : base(world, playerLeft, playerRight) {
            client = new Slim.SlimClientOfObject<PongGame>();
            client.DataReceived += OnRemoteDataReceived;
            PlayerRight.ObjectMoved += PlayerRight_ObjectMoved;
        }

        public void ConnectToServer(IPAddress ip, int port) {
            client.Connect(ip, port);
        }

        public void DisconnectFromServer() {
            client.Disconnect();
        }

        private void OnRemoteDataReceived(object sender, DataReceivedEventArgs<PongGame> args) {
            // if we received remote data from the server, we need to update our current state
            Ball.MoveTo(args.Data.Ball.Position);
            PlayerLeft.MoveTo(args.Data.PlayerLeft.Position);
        }

        private void PlayerRight_ObjectMoved(object sender, ObjectMovedEventArgs args) {
            // the player right is the client always. so we ned to report our position to the server
            client.Send(this);
        }

        public override void StartGame() {
            
            // we are overriding this method, to prevent us from starting the ball timer

            if (started) return;
            started = true; // set before everything else for cross thread safety
            PlayerLeft.Start();
            PlayerRight.Start();
        }

    }
}
