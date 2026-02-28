using UnityEngine;

namespace Physics
{
    public sealed class Controller : MonoBehaviour
    {
        [SerializeField] private Body _body;
        [SerializeField] private Contacts _contacts;
        [SerializeField] private Focus _focus;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _speed;

        private Vector3 _input;
        public Vector3 Input
        {
            get => _input;
            set => _input = value;
        }

        public Vector3 GetRelativeVector(Vector3 input)
        {
            Vector3 forward = (_focus.Forward + _focus.Up).normalized;
            Vector3 right = (_focus.Right).normalized;
            Vector3 relative = forward * input.z + right * input.x;
            return relative;
        }

        private void Update()
        {
            Input = GetRelativeVector(new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical")));
            Input = Input.Project(_contacts.Normal, keepMagnitude: true);
        }

        private void FixedUpdate()
        {
            _body.AddForce(Input * _acceleration * Time.deltaTime, magnitude: _speed, forceType: ForceType.Local);
        }
    }
}