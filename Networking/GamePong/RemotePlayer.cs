using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.GamePong {

    /// <summary>
    /// This is a dummy player that is controlled by a remote endpoint.
    /// It is dummy in the sense that does not rely in local user input.
    /// </summary>
    public class RemotePlayer : Player {

        public RemotePlayer(World world, WorldSide playerSide) : base(world, playerSide) {
        }

        public override void Start() {
            return; // dummy player
        }

        public override void Stop() {
            return; // dummy player
        }

    }

}
