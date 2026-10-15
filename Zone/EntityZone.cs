using UnityEngine;

namespace Physics
{
    public class EntityZone : Zone
    {
        [SerializeField] protected Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;
    }
}