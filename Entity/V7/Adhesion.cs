using Core.Numerics;
using Core.Tools;
using UnityEngine;

namespace Physics
{
    public sealed class Adhesion : MonoBehaviour
    {
        [SerializeField] [Range(-1f, 1f)] private float _adhesionDotMaximum = -0.71f;
        [SerializeField] [Range(0f, 1f)] private float _transitionDotMinimum = 0.71f;
        [SerializeField] private Contacts _contacts;
        [SerializeField] private bool _rotateWithNormal;

        public float AdhesionDotMaximum
        {
            get => _adhesionDotMaximum;
            set => _adhesionDotMaximum = value;
        }

        private bool PointValidation(Contact contact)
        {
            Debug.Log($"Validation {contact.Normal}  {contact.Normal.Dot(_contacts.Normal)}  {_contacts.Normal}");
            return contact.Normal.Dot(_contacts.Normal) >= _transitionDotMinimum;
        }

        private void Awake() => _contacts.Points.Validation = PointValidation;


        private void FixedUpdate()
        {
            if (!_rotateWithNormal) return;
            if (_contacts.Count <= 0) transform.up = -UnityEngine.Physics.gravity.normalized;
            else transform.up = _contacts.Normal;
        }

        public Vector3 GetAdhesiveForce(out float magnitude)
        {
            magnitude = 0;
            if (_contacts.Count <= 0) return Vector3.zero;

            float dot = Vector3.Dot(_contacts.Normal, UnityEngine.Physics.gravity.normalized);
            if (dot > AdhesionDotMaximum) return Vector3.zero;

            Vector3 force = -UnityEngine.Physics.gravity - _contacts.Normal * 2f;
            magnitude = force.magnitude;
            return force * Time.fixedDeltaTime;
        }
    }
    
}