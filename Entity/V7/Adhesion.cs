using Core.Numerics;
using Core.Tools;
using UnityEngine;

namespace Physics
{
    [RequireComponent(typeof(Influences.Contacts))]
    public sealed class Adhesion : MonoBehaviour
    {
        /// <summary>
        /// maxium dot angle from gravity direction object can move
        /// </summary>
        [SerializeField] [Range(-1f, 1f)] private float _adhesionDotMaximum = -0.71f;

        /// <summary>
        /// maximum dot angle between normals object can move across
        /// </summary>
        [SerializeField] [Range(0f, 1f)] private float _transitionDotMinimum = 0.71f;

        [SerializeField] private Influences.Contacts _contacts;
        [SerializeField] private bool _rotateWithNormal;

        #if UNITY_EDITOR
        private void Reset()
        {
            TryGetComponent(out _contacts);
        }
        #endif

        public float AdhesionDotMaximum
        {
            get => _adhesionDotMaximum;
            set => _adhesionDotMaximum = value;
        }

        private bool PointValidation(Influences.Contact contact)
        {
            return contact.Normal.Dot(_contacts.Normal) >= _transitionDotMinimum;
        }

        private void Awake() => _contacts.Influencers.Validation = PointValidation;


        private void FixedUpdate()
        {
            if (!_rotateWithNormal) return;
            if (_contacts.Influencers.Count <= 0) transform.up = -UnityEngine.Physics.gravity.normalized;
            else transform.up = _contacts.Normal;
        }

        public Vector3 GetAdhesiveForce(out float magnitude)
        {
            magnitude = 0;
            if (_contacts.Influencers.Count <= 0) return Vector3.zero;

            float dot = Vector3.Dot(_contacts.Normal, UnityEngine.Physics.gravity.normalized);
            if (dot > AdhesionDotMaximum) return Vector3.zero;

            Vector3 force = -UnityEngine.Physics.gravity - _contacts.Normal * 2f;
            magnitude = force.magnitude;
            return force * Time.fixedDeltaTime;
        }
    }
    
}