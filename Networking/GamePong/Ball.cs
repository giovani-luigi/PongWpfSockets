using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace CommonLib.GamePong {

    public class SideWallHitEventArgs : EventArgs {
        public WorldSide side; // side where the ball scaped
    }

    /// <summary>
    /// Represents the main ball for the Pong game.
    /// </summary>
    public class Ball : WorldObject {

        public delegate void HitSideWallsEventHandler(object sender, SideWallHitEventArgs e);

        public event HitSideWallsEventHandler SideWallHit;

        Random random = new Random(); // uses default seed

        private Vector2 _direction = Vector2.Zero;

        public bool Paused { get; set; } = false;

        private Vector2 Direction { 
            get { return _direction; } 
            set {
                _direction = value;
                Vector2.Normalize(_direction); // enforce normalization
            }
        }

        /// <summary>
        /// This method generates a random <see cref="Vector2{T}">2D Vector</see> that is
        /// within a given range specified in radians. 
        /// </summary>
        /// <remarks>The angle starting point is implementation dependent</remarks>
        /// <param name="angle">Angle (in radians) for how big the range of generated values should be</param>
        /// <param name="offset">Offset angle (in radians)</param>
        private Vector2 RandomVector2(float angle, float offset) {
            float r = (float)random.NextDouble() * angle + offset;
            return new Vector2(MathF.Cos(r), MathF.Sin(r));
        }

        public Ball(World world) : base(world, world.BallSize, world.BallSize) {
            ResetPosition();
        }

        public void ResetPosition() {
            
            MoveToCenter();

            // generates a random bounce to begin the game choosing 
            // randomly between left/right

            // generate an angle within [-angleRange, +angleRange]:
            float angleRange = MathF.PI / 6f; // pi/6 = 30deg
            float angle = ((float)random.NextDouble() * angleRange * 2f) - angleRange;
            Direction = Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(angle));

            // choose whether we flip direction (choose right/left)
            if (random.NextDouble() < 0.5F) {
                Direction = Vector2.Negate(Direction);
            }
        }

        public void MoveToCenter() {
            // set the initial position of the ball to the center of the world
            Position = new Point(
                (World.WorldWidth - World.BallSize) / 2,
                (World.WorldHeight - World.BallSize) / 2);
        }

        private void MoveNext() {

            // convert the current position to a vector so we can manipulate it
            Vector2 v = new Vector2(Position.X, Position.Y);

            // move the ball by some amount indicated, using the direction vector
            v += (Direction * World.BallStepSize);

            // update our position from the Vector
            Position = new PointF(v.X, v.Y);
        }

        /// <summary>
        /// Moves the ball to the next step, verifying collision with
        /// other game objects.
        /// </summary>
        /// <param name="worldObjects">The game objects to test for
        /// collisions</param>
        public void MoveNextBounceOn(IEnumerable<WorldObject> worldObjects) {

            if (Paused) return;

            // move to next position
            MoveNext();

            // check for collisions against other objects
            foreach (WorldObject obj in worldObjects) {
                if (obj.CollidesWith(this)) {
                    Bounce(obj.GetNormal());
                    return; // prevent further action
                }
            }

            // check for collisions against the world (vertical) boundaries
            if (this.Position.Y >= (World.WorldHeight - this.Height)) { // floor
                Bounce(new Vector2(0, -1));
                return; // prevent further action
            } else if (this.Position.Y <= 0) { // ceiling
                Bounce(new Vector2(0, 1));
                return; // prevent further action
            }

            // if we did NOT bounce we test for when ball goes outside the board

            // check for collistions against the side walls
            if (this.Position.X >= (World.WorldWidth - World.PadDistanceFromEdge - World.PadWidth)) {
                SideWallHit?.Invoke(this, new SideWallHitEventArgs() { side = WorldSide.Right } );
            } else if (this.Position.X <= (World.PadDistanceFromEdge + World.PadWidth)) {
                SideWallHit?.Invoke(this, new SideWallHitEventArgs() { side = WorldSide.Left });
            }
        }

        /// <summary>
        /// Bounce the ball using the reference as the normal for reflection.
        /// </summary>
        /// <param name="normal">The normal vector for reflection</param>
        /// <param name="remainingStepSize">Step size to be moved after reflection</param>
        private void Bounce(Vector2 normal) {

            if (normal == Vector2.Zero) { // generate 'random' direction
                
                // start from horizontal axis
                Vector2 newDirection = Vector2.UnitX;

                // rotate by a random amount
                float angle = (float)random.NextDouble() * 2 * MathF.PI;
                Vector2.Transform(newDirection, Matrix3x2.CreateRotation(angle));
                
                Direction = newDirection;
            } else { // generate a new direction based in the supplied direction
                     // if ref direction provided, we use it to calculate new value
                Direction = Vector2.Reflect(Direction, normal);
            }

            // move immediately so we can avoid unexpected behavior (like trapping the ball inside the pad)
            MoveNext();
        }

    }
}
