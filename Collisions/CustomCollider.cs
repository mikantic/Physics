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
        public Ratio Friction { get; }
        public Ratio Bounce { get; }
        public Vector3 Force { get; }
    }

    
}