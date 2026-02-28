using Core.Tools;
using Physics.Numerics;
using UnityEngine;

namespace Physics
{
    public struct Force
    {
        public Vector3 Acceleration;
        public float MaxMagnitude;

        public Force(Vector3 acceleration, float maxMagnitude = 50f)
        {
            Acceleration = acceleration;
            MaxMagnitude = maxMagnitude;
        }

        public void ValidateWithMagnitude(Vector3 velocity)
        {
            Acceleration = Acceleration.Adjust(velocity, MaxMagnitude);
        }

        public static Force operator -(Force force)
        {
            return new Force(
                acceleration: -force.Acceleration,
                maxMagnitude: force.MaxMagnitude
            );
        }

        public static Force operator +(Force a, Force b)
        {
            return new Force(
                acceleration: a.Acceleration + b.Acceleration,
                maxMagnitude: Mathf.Max(a.MaxMagnitude, b.MaxMagnitude)
            );
        }

        public static Force operator -(Force a, Force b)
        {
            return new Force(
                acceleration: a.Acceleration - b.Acceleration,
                maxMagnitude: Mathf.Max(a.MaxMagnitude, b.MaxMagnitude)
            );
        }

        public static Force operator *(Force force, float scaler)
        {
            return new Force(
                acceleration: force.Acceleration * scaler,
                maxMagnitude: force.MaxMagnitude
            );
        }

        public static Force Gravity = new Force(Vector3.down * 20f * Time.fixedDeltaTime);
        public static Force Empty = new Force(Vector3.zero, 0);
    }
}