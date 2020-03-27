using System;
using System.Drawing;
using System.Threading;
using System.Windows.Input;

// we are extending an external namespace here
namespace CommonLib.GamePong {

    /// <summary>
    /// This class represents a player that is controlled through the keyboard input.
    /// </summary>
    public class KeyboardPlayer : Player {

        private Thread keyListener;

        private CancellationTokenSource cancellation;

        private ManualResetEventSlim threadBlocker;

        private readonly Key MoveUpKey;
        private readonly Key MoveDownKey;

        public KeyboardPlayer(World world, WorldSide playerSide, Key moveUp, Key moveDown) 
            : base(world, playerSide) {

            // create thread object
            keyListener = new Thread(KeyListenerCallback);
            keyListener.SetApartmentState(ApartmentState.STA);

            // create cancellation source object
            cancellation = new CancellationTokenSource();

            // create semaphore to block the thread and prevent input
            threadBlocker = new ManualResetEventSlim(true);

            // store the keys that we need to listen to
            MoveUpKey = moveUp;
            MoveDownKey = moveDown;
        }

        private void KeyListenerCallback(Object parameter) {
            CancellationToken cancelToken = (CancellationToken)parameter;
            while (!cancelToken.IsCancellationRequested) {

                int delta = 0;

                // check which key is pressed and increment position
                if (Keyboard.IsKeyDown(MoveUpKey)) {
                    delta -= World.PadStepSize;  // subtract to go up in the screen coordinates system
                } else if (Keyboard.IsKeyDown(MoveDownKey)) {
                    delta += World.PadStepSize;  // add to go down in the screen coordinates system
                }

                // update the position of the object in the world
                if (delta != 0) {
                    Position = new PointF(Position.X, Position.Y + delta);
                }

                // wait some delay before checking again
                Thread.Sleep(World.KeyboardPollingIntervalMs);

                // block thread if player is suspended or we are waiting our next iteration timeout
                try {
                    threadBlocker.Wait(cancelToken);
                } catch (OperationCanceledException) {
                    return; // exit thread has been requested.
                }
            }
        }

        public override void Start() {
            keyListener.Start(cancellation.Token);
        }

        /// <summary>
        /// Signals the thread to come to a stop, and then block until it ends.
        /// </summary>
        public override void Stop() {
            cancellation.Cancel();
            if (keyListener.IsAlive) keyListener.Join();
        }

        /// <summary>
        /// Pause the game and all its background tasks
        /// </summary>
        public new void Suspend() {
            base.Suspend();
            threadBlocker.Reset();
        }

        /// <summary>
        /// Resume the game and all background tasks
        /// </summary>
        public new void Resume() {
            base.Resume();
            threadBlocker.Set();
        }

    }
}
