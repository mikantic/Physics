using Core.Tools;
using UnityEngine;

namespace Physics.Forces
{
    public static class ForceExtensions
    {
        public static double GetFriction(object value)
        {
            if (value is IFriction friction) return friction.Friction;
            return 0;
        }

        public static double GetBounce(object value)
        {
            if (value is IBounce bounce) return bounce.Bounce;
            return 0;
        }
    }

    public interface IFriction
    {
        public Ratio Friction { get; }
    }

    public interface IBounce
    {
        public Ratio Bounce { get; }
    }

    public interface IForce
    {
        public Vector3 Force { get; }
    }
}