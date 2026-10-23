using System.Collections.Generic;
using System.Linq;
using Core.Numerics;
using Saveur;
using UnityEngine;

namespace Physics
{ 
    public abstract class Data
    {
        public Vector3 Velocity { get; private set; }
        public Vector3 PreviousVelocity { get; private set; }

        public Data()
        {
            Velocity = Vector3.zero;
            PreviousVelocity = Vector3.zero;
        }

        public bool UpdateVelocity(Rigidbody rigidbody, out Vector3 delta)
        {
            delta = Vector3.zero;
            if (rigidbody == null) return false;
            PreviousVelocity = Velocity;
            Velocity = GetVelocity(rigidbody);
            delta = Velocity - PreviousVelocity;
            return true;
        }

        protected virtual Vector3 GetVelocity(Rigidbody rigidbody) => rigidbody.linearVelocity;
    }

    public class Contact : Data
    {
        public Vector3 Point { get; private set; }
        public Vector3 Normal { get; private set; }
        public float Friction { get; private set; }
        public float Bounciness { get; private set; }
        public Contact(Collision collision) : base()
        {
            Update(collision);
            if (collision.gameObject.TryGetComponent(out SurfaceProperties properties))
            {
                Friction = properties.Friction;
                Bounciness = properties.Bounciness;
            }
        }

        public void Update(Collision collision)
        {
            Point = Vector3.zero;
            Normal = Vector3.zero;
            if (collision.contactCount <= 0) return;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Point += collision.contacts[i].point;
                Normal += collision.contacts[i].normal;
            }
            Point /= collision.contactCount;
            Normal = (Normal / collision.contactCount).normalized;
        }

        protected override Vector3 GetVelocity(Rigidbody rigidbody)
        {
            return rigidbody.GetPointVelocity(Point);
        }
    }

    public static class CollisionExtensions
    {
        public static float DynamicFriction(this Collider collider)
        {
            PhysicsMaterial material = collider.sharedMaterial;
            if (material == null) return 0;
            return material.dynamicFriction;
        }

        public static float StaticFriction(this Collider collider)
        {
            PhysicsMaterial material = collider.sharedMaterial;
            if (material == null) return 0;
            return material.staticFriction;
        }

        public static float Bounciness(this Collider collider)
        {
            PhysicsMaterial material = collider.sharedMaterial;
            if (material == null) return 0;
            return material.bounciness;
        }
    }
}