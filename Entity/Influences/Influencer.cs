using Physics.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public abstract class Influencer
    {
        public readonly Vector3 Point;

        private readonly Rigidbody _rigidbody;
        public Vector3 Velocity(Vector3 position)
        { 
           return _rigidbody?.GetPointVelocity(position) ?? Vector3.zero;
        }

        protected Influencer(Vector3 point, GameObject gameObject)
        {
            Point = point;
            gameObject.TryGetComponent(out _rigidbody);
        }
    }
}