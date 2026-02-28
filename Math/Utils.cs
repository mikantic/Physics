using Core.Numerics;
using UnityEngine;

namespace Physics
{
    public static class Utils
    {
        public static Contact ToContact(this Collision collision) => new Contact(collision);

        public static bool InBounds(this ref Vector3 force, float magnitude, Vector3 velocity)
        {
            float delta = velocity.InDirection(force) - magnitude;
            if (delta >= 0) return false;

            float overflow = delta + force.magnitude;
            if (overflow > 0) force = force.normalized * (force.magnitude - overflow);

            return true;
        }

        public static Vector3 Project(this Vector3 vector, Vector3 normal, bool keepMagnitude = false)
        {
            Vector3 projected = Vector3.ProjectOnPlane(vector, normal);
            if (!keepMagnitude) return projected;
            return projected.normalized * vector.magnitude;
        }
    }
}