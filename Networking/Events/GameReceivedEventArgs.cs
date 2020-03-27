using System;
using CommonLib.GamePong;

namespace CommonLib.Events {

    /// <summary>
    /// Argument to be used when a game object is received from some source
    /// </summary>
    public class GameReceivedEventArgs : EventArgs {

        /// <summary>
        /// The received <see cref="PongGame"/>
        /// </summary>
        public PongGame Game { get; }

        public GameReceivedEventArgs(PongGame game) {
            Game = game;
        }

    }
}
