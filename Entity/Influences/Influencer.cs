using Physics.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public abstract class Influencer
    {
        private readonly Vector3 _point;
        public Vector3 Point {get => _point; }

        private readonly Rigidbody _rigidbody;
        public Vector3 Velocity 
        { 
            get
            {
                if (_rigidbody == null) return Vector3.zero;
                return _rigidbody.GetPointVelocity(Point);
            }
        }

        protected Influencer(Vector3 point, GameObject gameObject)
        {
            _point = point;
            gameObject.TryGetComponent(out _rigidbody);
        }
    }
}