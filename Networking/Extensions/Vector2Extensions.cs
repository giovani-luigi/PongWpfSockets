using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CommonLib.Extensions {
    public static class Vector2Extensions {

        /// <summary>
        /// AngleBetweenDeg - the angle between 2 vectors (output in degrees)
        /// </summary>
        /// <returns>
        /// Returns the the angle in degrees between vector1 and vector2
        /// </returns>
        /// <param name="vector1"> The first Vector </param>
        /// <param name="vector2"> The second Vector </param>
        public static float AngleBetweenDeg(this Vector2 vector1, Vector2 vector2) {
            return AngleBetweenRad(vector1, vector2) * (180 / MathF.PI);
        }

        /// <summary>
        /// AngleBetweenRad - the angle between 2 vectors (output in Radians)
        /// </summary>
        /// <returns>
        /// Returns the the angle in radians between vector1 and vector2
        /// </returns>
        /// <param name="vector1"> The first Vector </param>
        /// <param name="vector2"> The second Vector </param>
        public static float AngleBetweenRad(this Vector2 vector1, Vector2 vector2) {
            float sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            float cos = vector1.X * vector2.X + vector1.Y * vector2.Y;
            return MathF.Atan2(sin, cos) ;
        }

    }
}
