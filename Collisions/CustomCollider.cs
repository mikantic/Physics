using Core.Tools;
using Physics.Forces;
using UnityEngine;

namespace Physics.Colliders
{
    public interface ICollider : IFriction, IBounce, IForce
    {
        
    }
    
    [RequireComponent(typeof(Transform))]
    public class CustomCollider : MeshCollider, ICollider
    {
        public Range01 Friction { get; }
        public Range01 Bounce { get; }
        public Vector3 Force { get; }
    }

    
}