using Core.Tools;
using Physics.Colliders;
using Physics.Forces;
using UnityEngine;

namespace Physics.Entities
{
    public class Entity : MonoBehaviour
    {
        public Vector3 Velocity { get; protected set; }

        protected virtual void OnCollisionBegin(Collision collision, ICollider collider)
        {
            
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider is not ICollider collider) return;
            OnCollisionBegin(collision, collider);
        }

        protected virtual void OnCollisionContinue(Collision collision, ICollider collider)
        {
            
        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider is not ICollider collider) return;
            OnCollisionContinue(collision, collider);
        }

        protected virtual void OnCollisionEnd(ICollider collider)
        {
            
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider is not ICollider collider) return;
            OnCollisionEnd(collider);   
        }
    }
}