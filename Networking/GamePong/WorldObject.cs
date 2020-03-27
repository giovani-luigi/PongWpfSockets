using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace CommonLib.GamePong {

    public class ObjectMovedEventArgs : EventArgs {
        public PointF Position;
    }

    public delegate void ObjectMovedEventHandler(object sender, ObjectMovedEventArgs args);

    /// <summary>
    /// This class represents an object in the game that is aware of the world boundaries.
    /// It serves as base class for other implementations like players and other obstacles.
    /// </summary>
    public class WorldObject : GameObject {

        /// <summary>
        /// Event raised when the position of the object changes
        /// </summary>
        public event ObjectMovedEventHandler ObjectMoved;

        public World World;
        
        public new virtual PointF Position {
            get { return base.Position; }
            set {
                // calculate position limits for the object
                int minX = 0, minY = 0;
                int maxX = World.WorldWidth - Width;
                int maxY = World.WorldHeight - Height;
                // clamp position inside the world bounds
                value.X = Math.Min(Math.Max(minX, value.X), maxX);
                value.Y = Math.Min(Math.Max(minY, value.Y), maxY);
                // if position changed, notify all listeners
                if (value != base.Position) {
                    base.Position = value;
                    ObjectMoved?.Invoke(this, new ObjectMovedEventArgs() { Position = Position });
                }
            }
        }

        internal void MoveTo(PointF position) {
            Position = position;
        }

        public WorldObject(World world, int width, int height) : base(width, height) {
            World = world;
        }

        public virtual Vector2 GetNormal() {
            return Vector2.Zero;
        }

    }
}
