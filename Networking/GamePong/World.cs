using System.Drawing;

namespace CommonLib.GamePong {

    public enum WorldSide {
        Left = -1,
        Right = 1
    }

    /// <summary>
    /// This class contains a set of values to be used as the 
    /// rules common to all entities in the game.
    /// </summary>
    public class World {

        // Size of the region where the ball will move
        public int WorldWidth { get; } = 800;
        public int WorldHeight { get; } = 480;

        // Size of the pads that will bounce the ball
        public int PadWidth { get; } = 10; // try to avoid being bigger than ball diameter
        public int PadHeight { get; } = 80;

        // Position of the pads
        public int PadInitialPosY { get; } = (480 - 80) / 2;
        public int PadEdgeDistance { get; } = 20;

        // distance from the screen edge edge
        public int PadDistanceFromEdge { get; } = 20;

        /// <summary>
        /// The amount of pixels the pad is moved every time a key up/down is detected
        /// </summary>
        public int PadStepSize { get; } = 10;

        /// <summary>
        /// The diameter of the ball
        /// </summary>
        public int BallSize { get; } = 10;

        /// <summary>
        /// Distance to move the ball on every iteration
        /// </summary>
        /// <remarks> Keep smaller than Pad width to avoid tunneling 
        /// through when at high speed</remarks>
        public int BallStepSize { get; } = 5;

        /// <summary>
        /// Interval between ball iterations
        /// </summary>
        public int BallIntervalMs { get; } = 25;

        /// <summary>
        /// Interval between keyboard button polling
        /// </summary>
        public int KeyboardPollingIntervalMs { get; } = 50;
    }
}
