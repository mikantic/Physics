using UnityEngine;

namespace Physics
{
    public sealed class Controller : MonoBehaviour
    {
        [SerializeField] private Body _body;
        [SerializeField] private Influences.Contacts _contacts;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _speed;

        private Vector3 _input;
        public Vector3 Input
        {
            get => _input;
            set => _input = value;
        }
    }
}