using CommonLib.GamePong;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Communication {

    /// <summary>
    /// Implements a creational pattern for the network packet.
    /// This is a mix of Factory and Builder pattern to generate
    /// objects <see cref="NetworkPacket"/>
    /// </summary>
    /// <remarks>
    /// Even though there is no inheritance for now, this factory
    /// is still useful to isolate the logic to build packets from
    /// world objects or any other input object.
    /// </remarks>
    public class NetworkPacketFactory {

        private readonly NetworkPacket packet;

        public NetworkPacketFactory() {
            packet = new NetworkPacket();
        }

        /// <summary>
        /// Builds the final <see cref="NetworkPacket"/>
        /// </summary>
        public NetworkPacket Build() {
            return packet;
        }

        /// <summary>
        /// Add by cloning (deep copy) the object data.
        /// </summary>
        public NetworkPacketFactory AddPlayers(Player p) {
            // clone derived class to a more basic class, so we have a small object to serialize
            if (p.PlayerSide == WorldSide.Left) {
                packet.PlayerLeft = p.DeepClone();
                packet.PlayerLeftScore = p.Score;
            } else if (p.PlayerSide == WorldSide.Right) {
                packet.PlayerRight = p.DeepClone();
                packet.PlayerRightScore = p.Score;
            }
            return this;
        }

        /// <summary>
        /// Add by cloning (deep copy) the object data.
        /// </summary>
        public NetworkPacketFactory AddBall(Ball b) {
            // clone derived class to a more basic class, so we have a small object to serialize
            packet.Ball = b.DeepClone(); 
            return this;
        }

        public NetworkPacketFactory AddObjects(IEnumerable<GameObject> objects) {
            foreach (var o in objects) {
                if (o is Player p) { AddPlayers(p); continue; }
                if (o is Ball b) { AddBall(b); continue; }
            }
            return this;
        }

        
    }
}
