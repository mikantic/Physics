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
        public Vector3 Normal => Points.Data.Values.Select(contact => contact.Normal).Average(
            emptyResult: -UnityEngine.Physics.gravity.normalized);
            
        public Vector3 Velocity => Points.Data.Values.Select(contact => contact.Velocity).Sum();

        public int Count => Points.Data.Count;

        private void OnCollisionEnter(Collision collision) => Points[collision.collider] = collision.ToContact();
        private void OnCollisionStay(Collision collision) => Points[collision.collider] = collision.ToContact();
        private void OnCollisionExit(Collision collision) => Points.Remove(collision.collider);
    }
}