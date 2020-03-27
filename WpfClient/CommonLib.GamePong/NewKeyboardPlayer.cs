using System;
using System.Drawing;
using System.Threading;
using System.Windows.Input;
using WpfClient.Controls;

// we are extending an external namespace here
namespace CommonLib.GamePong {

    /// <summary>
    /// This class represents a player that is controlled through the keyboard input.
    /// </summary>
    public class NewKeyboardPlayer : Player {
        
        private readonly Key MoveUpKey;
        private readonly Key MoveDownKey;
        private readonly WorldView Host;

        /// <summary>
        /// Creates a new game player (must be either the left or the right pad player)
        /// </summary>
        /// <param name="world">The world rules</param>
        /// <param name="playerSide">The side of this player</param>
        /// <param name="moveUp">key to use for move-up</param>
        /// <param name="moveDown">key to use for move-down</param>
        /// <param name="host">The user control used to host this player. 
        /// Will be used to hook mouse events</param>
        /// <remarks>Call this when UI is ready, so not inside a control constructor</remarks>
        public NewKeyboardPlayer(World world, WorldSide playerSide, Key moveUp, Key moveDown, WorldView host)
            : base(world, playerSide) {

            // store the keys that we need to listen to
            MoveUpKey = moveUp;
            MoveDownKey = moveDown;
            Host = host;

            Keyboard.Focus(host);
        }

        private void OnKeyDown(object sender, KeyEventArgs e) {

            int delta = 0;
            
            // check which key is pressed and increment position
            if (e.Key == MoveUpKey) {
                delta -= World.PadStepSize;  // subtract to go up in the screen coordinates system
                e.Handled = true;
            } else if (e.Key == MoveDownKey) {
                delta += World.PadStepSize;  // add to go down in the screen coordinates system
                e.Handled = true;
            }

            // update the position of the object in the world
            if (delta != 0) {
                Position = new PointF(Position.X, Position.Y + delta);
            }
        }

        public override void Start() {
            // hook the event handlers to the host
            Host.PreviewKeyDown += OnKeyDown;
        }

        /// <summary>
        /// Signals the thread to come to a stop, and then block until it ends.
        /// </summary>
        public override void Stop() {
            // unhook the event handlers to the host
            Host.PreviewKeyDown -= OnKeyDown;
        }
        
    }
}
