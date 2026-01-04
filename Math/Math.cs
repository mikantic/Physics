using UnityEngine;
using Core.Numerics;
using Core.Tools;

namespace Physics.Numerics
{
    public static class Math
    {
        public static Vector3 Collision(this Vector3 velocity, Vector3 normal, double bounce, double friction)
        {
            return (velocity - velocity.InDirection(normal.normalized) * (1 + (float)bounce)) * (float)(1 - friction);
        }
    }
}