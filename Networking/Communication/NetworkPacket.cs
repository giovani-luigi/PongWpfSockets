using CommonLib.GamePong;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib.Communication {
    
    /// <summary>
    /// Contains the data we want to transfer through sockets.
    /// This class contains a collection of all world objects
    /// in their most basic versions, i.e. with attributes like
    /// position in the screen and other simple data.
    /// </summary>
    [Serializable]
    public class NetworkPacket {

        public int PlayerLeftScore { get; set; }
        
        public int PlayerRightScore { get; set; }

        public GameObject Ball { get; set; }

        public GameObject PlayerLeft { get; set; }

        public GameObject PlayerRight { get; set; }

        [JsonConstructor]
        public NetworkPacket() { }
        
    }
}
