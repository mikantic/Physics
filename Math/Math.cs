using UnityEngine;
using Core.Numerics;
using Core.Tools;
using System.Linq;

namespace Physics.Numerics
{
    public sealed class Friction : FloatRange
    {
        public Friction(float value) : base(value, 0, 1) {}
    }

    public static class Math
    {
        public static float Friction(this Collider collider) => collider.material.dynamicFriction;

        public static FrictionType Reverse(this FrictionType frictionType)
        {
            return frictionType switch
            {
                FrictionType.Against => FrictionType.With,
                FrictionType.With or _ => FrictionType.Against
            };
        }

        public static Vector3 Along(
            this Vector3 vector, 
            Vector3 normal, 
            double maxDot = 0,
            double minDot = Core.Numerics.Math.DOT_135, 
            ForceMagnitude forceMagnitude = ForceMagnitude.Project)
        {
            float dot = vector.Dot(normal);
            if (dot >= maxDot) return vector;
            if (dot < minDot) 
            {
                return Vector3.zero;
            }
            if (forceMagnitude == ForceMagnitude.Maintain)
            {
                float magnitude = vector.magnitude;
                return Vector3.ProjectOnPlane(vector, normal).normalized * magnitude;
            }
            return Vector3.ProjectOnPlane(vector, normal);
        }




        public static float Bounce(this Collider collider) => collider.material.bounciness;
        public static Vector3 Normal(this Collision collision) => collision.contacts.Select(contact => contact.normal).Average();
        public static Vector3 Point(this Collision collision) => collision.contacts.Select(contact => contact.point).Average();

        public static Vector3 Adjust(this Vector3 force, Vector3 velocity, double maxMagnitude = 50)
        {
            float velocityMagnitude = velocity.InDirection(force);
            if (velocityMagnitude >= maxMagnitude) return Vector3.zero;
            if (velocityMagnitude + force.magnitude <= maxMagnitude) return force;
            return force.normalized * (float)(maxMagnitude - velocityMagnitude);
        }
    }
}