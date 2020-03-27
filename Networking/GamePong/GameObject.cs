using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CommonLib.GamePong {
    
    /// <summary>
    /// This class represents an object that is aware of 
    /// its position and sizes. This class should be extended
    /// to implement methods that will can manipulate the
    /// fields in this class.
    /// </summary>
    /// <remarks>
    /// This class was extracted from <see cref="WorldObject">
    /// to use in the network data exchange.
    /// </remarks>
    [JsonObject]
    public class GameObject {

        /// <summary>
        /// The position of the object in the canvas
        /// </summary>
        [JsonIgnore]
        public virtual PointF Position { 
            get {
                return new PointF(x, y);
            } set {
                x = value.X;
                y = value.Y;
            } 
        }

        [JsonProperty]
        private float x { get; set; }

        [JsonProperty]
        private float y { get; set; }

        [JsonProperty]
        public int Height { get; set; }

        [JsonProperty]
        public int Width { get; set; }

        [JsonConstructor]
        private GameObject() {}

        public GameObject(int width, int height) {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Returns the Axis Aligned Min. Bounding Box of the object
        /// </summary>
        public RectangleF GetBoundingBox() {
            return new RectangleF(Position, new Size(Width, Height));
        }

        /// <summary>
        /// Check whether an object collides with the other, using a very simple AABB test.
        /// </summary>
        public bool CollidesWith(GameObject other) {
            return GetBoundingBox().IntersectsWith(other.GetBoundingBox());
        }

        /// <summary>
        /// Provides a deep clone of the GameObject
        /// </summary>
        public GameObject DeepClone() {
            return new GameObject(Width, Height) { x = x, y = y }; // deep clone
        }

    }

}
