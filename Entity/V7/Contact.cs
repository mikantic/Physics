using System;
using Physics.Numerics;
using UnityEngine;

namespace Physics
{
    public readonly struct Contact
    {
        private readonly Vector3 _normal;
        public Vector3 Normal => _normal;

        private readonly Vector3 _point;
        public Vector3 Point => _point;

        private readonly Rigidbody _rigidbody;
        public Vector3 Velocity =>  _rigidbody?.GetPointVelocity(_point) ?? Vector3.zero;

        public Contact(
            Vector3 normal,
            Vector3 point,
            Rigidbody rigidbody)
        {
            _normal = normal.normalized;
            _point = point;
            _rigidbody = rigidbody;
        }

        public Contact(
            Collision collision)
        {
            _normal = collision.Normal().normalized;
            _point = collision.Point();
            collision.gameObject.TryGetComponent(out _rigidbody);
        }
    }
}