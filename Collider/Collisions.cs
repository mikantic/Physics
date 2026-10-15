using System.Collections.Generic;
using System.Linq;
using Core.Numerics;
using Saveur;
using UnityEngine;

namespace Physics
{ 
    public class CollisionData
    {
        public Vector3 Normal { get; private set; }
        public Vector3 Point { get; private set; }
        public Vector3 PointVelocity { get; set; }
        public Rigidbody Rigidbody { get; private set; } = null;

        public CollisionData(Collision collision)
        {
            Update(collision);
            PointVelocity = Vector3.zero;
            if (collision.collider.gameObject.layer.ToLayer() == Layer.Launch) return;
            Rigidbody = collision.collider.attachedRigidbody;
        }

        public void Update(Collision collision)
        {
            Point = Vector3.zero;
            Normal = Vector3.zero;

            int count = collision.contactCount;

            for (int i = 0; i < count; i++)
            {
                ContactPoint contactPoint = collision.GetContact(i);
                Point += contactPoint.point;
                Normal += contactPoint.normal;
            }

            Point /= count;
            Normal /= count;
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