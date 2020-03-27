using CommonLib.GamePong;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Communication {
    
    /// <summary>
    /// Contains the data we want to transfer through sockets
    /// </summary>
    public class NetworkPacket {

        public NetworkPacket(Player playerLeft, Player playerRight, Ball ball) {
            PlayerLeft = playerLeft;
            PlayerRight = playerRight;
            Ball = ball;
        }

        public Player PlayerLeft { get; }
        public Player PlayerRight { get; }
        public Ball Ball { get; }
    }
}
