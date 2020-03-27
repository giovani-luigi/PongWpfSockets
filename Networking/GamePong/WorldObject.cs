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

    [Serializable]
    public class WorldObject {

        /// <summary>
        /// Event raised when the position of the object changes
        /// </summary>
        public event ObjectMovedEventHandler ObjectMoved;

        public World World;

        public int Height { get; }

        public int Width { get; }

        private PointF _position;        

        public virtual PointF Position {
            get { return _position; }
            set {
                // calculate position limits for the object
                int minX = 0, minY = 0;
                int maxX = World.WorldWidth - Width;
                int maxY = World.WorldHeight - Height;
                // clamp position inside the world bounds
                value.X = Math.Min(Math.Max(minX, value.X), maxX);
                value.Y = Math.Min(Math.Max(minY, value.Y), maxY);
                // if position changed, notify all listeners
                if (value != _position) {
                    _position = value;
                    ObjectMoved?.Invoke(this, new ObjectMovedEventArgs() { Position = Position });
                }
            }
        }

        internal void MoveTo(PointF position) {
            Position = position;
        }

        public WorldObject(World world, int width, int height) {
            World = world;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Returns the Axis Aligned Min. Bounding Box of the object
        /// </summary>
        public RectangleF BoundingBox {
            get {
                return new RectangleF(_position, new Size(Width, Height));
            }
        }

        /// <summary>
        /// Check whether an object collides with the other, using a very simple AABB test.
        /// </summary>
        public bool CollidesWith(WorldObject other) {
            return BoundingBox.IntersectsWith(other.BoundingBox);
        }

        public virtual Vector2 GetNormal() {
            return Vector2.Zero;
        }

    }
}
