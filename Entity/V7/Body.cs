using Unity.VisualScripting;
using UnityEngine;

namespace Physics
{
    public sealed class Body : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Contacts _contacts;
        [SerializeField] private Adhesion _adhesion;

        public Vector3 GlobalVelocity => _rigidbody.linearVelocity;
        public Vector3 LocalVelocity => GlobalVelocity - _contacts.Velocity;

        private Vector3 Velocity(
            ForceType forceType = ForceType.Global)
        {
            return forceType switch
            {
                ForceType.Local => LocalVelocity,
                ForceType.Global or _ => GlobalVelocity
            };
        }

        public void AddForce(
            Vector3 force, 
            float magnitude, 
            ForceType forceType = ForceType.Global)
        {
            if (!force.InBounds(magnitude, Velocity(forceType))) return; 

            Debug.Log($"Add Force: {force}  Normal {_contacts.Normal}");  
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        private void FixedUpdate()
        {
            AddForce(
                force: _adhesion.GetAdhesiveForce(out float magnitude),
                magnitude: magnitude,
                forceType: ForceType.Global // might have to split it up to have anti gravity be global but idk
            );
        }
    }
}