using System.Collections.Generic;
using System.Linq;
using Core.Numerics;
using Core.Tools;
using UnityEngine;

namespace Physics
{
    public sealed class Contacts : MonoBehaviour
    {
        public Map<Collider, Contact> Points { get; } = new();

        private Vector3 _normal = Vector3.up;
        public Vector3 Normal
        {
            get => _normal;
        }
            
        public Vector3 Velocity => Points.Data.Values.Select(contact => contact.Velocity).Sum();

        public int Count => Points.Data.Count;

        private void RecalculateNormal()
        {
            if (Points.Count <= 0) 
            {
                _normal = UnityEngine.Physics.gravity.normalized;
                return;
            }

            _normal = Points.Data.Values.Select(contact => contact.Normal).Average(
                emptyResult: -UnityEngine.Physics.gravity.normalized);
        }

        private void UpdateData(Collision collision)
        {
            Points[collision.collider] = collision.ToContact();
            RecalculateNormal();
        }

        private void OnCollisionEnter(Collision collision) => UpdateData(collision);
        private void OnCollisionStay(Collision collision) => UpdateData(collision);
        private void OnCollisionExit(Collision collision) 
        {
            Points.Remove(collision.collider);
            RecalculateNormal();
        }
    }
}