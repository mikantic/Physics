using Physics.Influences;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Physics.Controls
{
    [RequireComponent(typeof(Body))]
    [RequireComponent(typeof(Contacts))]
    public class Controller : MonoBehaviour
    {
        [SerializeField] protected Body _body;
        [SerializeField] protected Contacts _contacts;
        [SerializeField] protected Volumes _volumes;
        [SerializeField] protected Camera _camera;
        [SerializeField] protected InputActionReference _input;

        [SerializeField] protected InputActionReference _jump;

        [SerializeField] protected float _acceleration;
        [SerializeField] protected float _magnitude;

        #if UNITY_EDITOR
        private void Reset()
        {
            TryGetComponent(out _body);
            TryGetComponent(out _contacts);
            TryGetComponent(out _volumes);
        }
#endif

        protected void Awake()
        {
            _jump.action.performed -= Jump;
            _jump.action.performed += Jump;
        }

        protected void Jump(InputAction.CallbackContext _)
        {
            if (_contacts.Influencers.Count <= 0) return;

            Vector3 force = -UnityEngine.Physics.gravity.normalized;
            force *= 12;
            _body.AddForce(force, force.magnitude, ForceType.Local);
        }

        protected Vector3 GetInputDirection(Vector2 input)
        {
            Vector3 force = (_camera.transform.forward * input.y + _camera.transform.right * input.x).normalized;
            return force.Project(_contacts.Normal);
        }

        protected void FixedUpdate()
        {   
            Vector2 input = _input.action.ReadValue<Vector2>();
            if (input.magnitude <= 0) return;

            Vector3 direction = GetInputDirection(input);
            if (direction.magnitude <= 0) return;

            float scaler = Mathf.Max(1 - _volumes.Density, 0.25f);
            _body.AddForce(direction * _acceleration * Time.fixedDeltaTime, _magnitude * scaler, ForceType.Local);            
        }

    }
}