using CommonLib.Server;
using System;

namespace Server.Pong {

    /// <summary>
    /// This class implements a server for the Pong game, with log output to the console.
    /// </summary>
    internal class PongServer : GameServer {

        internal PongServer() : base(Console.Out) {

        }

    }
}
