using UnityEngine;

namespace Physics
{
    public class Focus : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _matchVertical;

        public Vector3 Up { get => _matchVertical ? _target.up : transform.up; }
        public Vector3 Forward { get => transform.forward; }
        public Vector3 Right { get => transform.right; }

        private void Update()
        {
            transform.LookAt(_target, Up);
        }
    }    
}

