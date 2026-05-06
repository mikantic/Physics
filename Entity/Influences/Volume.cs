using System.Linq;
using Core.Numerics;
using Core.Tools;
using Physics.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public class Volume : Influencer
    {
        private readonly VolumeFlow _flow;
        public Vector3 Flow => _flow?.Flow ?? Vector3.zero;
        public float Density => _flow?.Density ?? 0;

        public Volume(Collider collider, Vector3 point) : base(point, collider.gameObject)
        {
            collider.gameObject.TryGetComponent(out _flow);
        }

        protected float GetDepth(Collider collider)
        {
            Vector3 direction = UnityEngine.Physics.gravity.normalized;
            Vector3 origin = Point - direction * 50;
            Ray ray = new Ray(origin, direction);
            if (collider.Raycast(ray, out RaycastHit hit, 100))
            {
                return (Point - hit.point).InDirection(direction);
            }
            return 0;
        }
        
        public Vector3 GetTensionForce(Vector3 velocity, out float magnitude)
        {
            magnitude = velocity.magnitude * Density / 2;
            return -velocity;
        }

        public Vector3 GetBuoyantForce(Collider collider, out float magnitude)
        {
            float depth = Mathf.Min(GetDepth(collider), 2f);
            Vector3 force = -UnityEngine.Physics.gravity;
            magnitude = force.magnitude;
            return force * depth * Density * Time.fixedDeltaTime;
        }
    }
}