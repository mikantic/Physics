using UnityEngine;
using Core.Numerics;
using System.Linq;

namespace Physics.Numerics
{
    public struct DotBounds
    {
        public float Nullified { get; }
        public float Projected { get; }
        private DotBounds(float nullified, float projected)
        {
            Nullified = nullified;
            Projected = projected;
        }
        public static DotBounds Default => new(-0.707f, 0f);
        public static DotBounds Project => new(-1f, 1f);
    }
    public static class Math
    {
        
        /// <summary>
        /// get a vector projected on a plane if it is able to be
        /// </summary>
        /// <param name="vector">input vector</param>
        /// <param name="normal">normal</param>
        /// <param name="minDot">lowest dot value until it returns 0</param>
        /// <param name="maxDot">largest dot value until it returns input vector</param>
        /// <param name="preserveMagnitude">if projected vector preserves input magnitude</param>
        /// <returns></returns>
        public static Vector3 Project(
            this Vector3 vector, 
            Vector3 normal, 
            float nullifiedDot = -.707f, 
            float projectedDot = 0f,
            bool preserveMagnitude = false)
        {
            float dot = vector.Dot(normal);
            if (dot < nullifiedDot) return Vector3.zero;
            if (dot > projectedDot) return vector;
            Vector3 result = Vector3.ProjectOnPlane(vector, normal);
            if (preserveMagnitude) return result.normalized * vector.magnitude;
            return result;
        }

        /// <summary>
        /// projects on plane with magnitude option
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <param name="preserveMagnitude"></param>
        /// <returns></returns>
        public static Vector3 Project(
            this Vector3 vector, 
            Vector3 normal, 
            bool preserveMagnitude = false // maybe make this enum? default, perserve, normalize?
        )
        {
            Vector3 result = Vector3.ProjectOnPlane(vector, normal);
            if (preserveMagnitude) return result.normalized * vector.magnitude;
            return result;
        }

        /// <summary>
        /// returns a vector clamped to a magnitude compared to a reference vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="magnitude"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Vector3 Clamp(this Vector3 vector, float magnitude, Vector3 reference)
        {
            float inDirection = reference.InDirection(vector);
            if (inDirection >= magnitude) return Vector3.zero;

            float remaining = magnitude - inDirection;
            if (remaining < vector.magnitude) return remaining * vector.normalized;

            return vector;
        }
    }
}