using Unity.VisualScripting;
using UnityEngine;

namespace Physics
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Influences.Contacts))]
    [RequireComponent(typeof(Influences.Volumes))]
    [RequireComponent(typeof(Adhesion))]
    public sealed class Body : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Influences.Contacts _contacts;
        [SerializeField] private Influences.Volumes _volumes;
        [SerializeField] private Adhesion _adhesion;

        [Range(0,1)]
        [SerializeField] private float _buoyancy = 0.75f;

        public Vector3 GlobalVelocity => _rigidbody.linearVelocity;
        public Vector3 LocalVelocity => GlobalVelocity - _contacts.Velocity;

        #if UNITY_EDITOR
        private void Reset()
        {
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _contacts);
            TryGetComponent(out _adhesion);
            TryGetComponent(out _volumes);
        }
        #endif

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
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        private void FixedUpdate()
        {
            AddForce(
                force: _adhesion.GetAdhesiveForce(out float adhesiveMagnitude),
                magnitude: adhesiveMagnitude,
                forceType: ForceType.Global 
            );

            AddForce(
                force: _contacts.Velocity,
                magnitude: _contacts.Velocity.magnitude,
                forceType: ForceType.Global
            );

            AddForce(
                force: _volumes.GetBuoyantForce(out float buoyantMagnitude) * _buoyancy,
                magnitude: buoyantMagnitude,
                forceType: ForceType.Global
            );

            AddForce(
                force: _volumes.GetFlowForce(out float flowMagnitude),
                magnitude: flowMagnitude,
                forceType: ForceType.Global
            );
        }
    }
}