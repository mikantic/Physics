using System.Collections.Generic;
using Core.Numerics;
using Physics.Numerics;
using Saveur;
using UnityEngine;
namespace Physics
{
    public enum FrictionMode
    {
        Against, With, None
    }

    public enum DurationMode
    {
        Instant, Continuous
    }

    public enum ReferenceMode
    {
        Local, Global
    }


    public partial class Body : MonoBehaviour
    {
        [SerializeField] protected Rigidbody body;
        protected Dictionary<Collider, CollisionData> contacts { get; } = new();

        protected Vector3 contactVelocity;
        protected Vector3 localVelocity => body.linearVelocity - contactVelocity;

        protected float minimumValue = 0.001f;

        protected virtual void UpdateContactVelocity()
        {
            Vector3 velocity = Vector3.zero;
            int count = 0;
            foreach (var (collider, data) in contacts)
            {
                if (data.Rigidbody == null) continue;
                velocity += data.PointVelocity;
                count++;
            }
            if (count <= 0) contactVelocity = Vector3.zero;
            else contactVelocity = velocity / count;
        }

        public virtual void AddForce(
            Vector3 force,
            float magnitude,
            DurationMode durationMode = DurationMode.Instant,
            FrictionMode frictionMode = FrictionMode.Against,
            ReferenceMode referenceMode = ReferenceMode.Global,
            float nullifiedDot = -.707f,
            float projectedDot = 0f,
            bool preserveMagnitude = false,
            bool requireContacts = false
        )
        {
            // contact mode
            float maxFriction = 0;
            int count = 0;
            foreach (var (collider, data) in contacts)
            {
                float dot = force.Dot(data.Normal);
                if (dot > projectedDot) continue;
                if (dot < nullifiedDot) return;
                float friction = collider.DynamicFriction();
                if (friction > maxFriction) maxFriction = friction;
                force = force.Project(data.Normal, preserveMagnitude);
                count++;
            }
            if (requireContacts && count <= 0) return;
            if (force.magnitude < minimumValue) return;

            // friction mode
            if (frictionMode != FrictionMode.None)
            {
                force *= frictionMode switch
                {
                    FrictionMode.With => maxFriction,
                    FrictionMode.Against or _ => 1f - maxFriction
                };
                if (force.magnitude < minimumValue) return;
            }

            // duration mode 
            if (durationMode == DurationMode.Continuous)
            {
                force *= Time.fixedDeltaTime;
            }
            if (force.magnitude < minimumValue) return;

            // reference mode 
            force = force.Clamp(magnitude, referenceMode switch
            {
                ReferenceMode.Local => localVelocity,
                ReferenceMode.Global or _ => body.linearVelocity 
            });
            if (force.magnitude < minimumValue) return;

            Debug.Log($"Force Applied {force}");
            // apply force
            body.AddForce(force, ForceMode.VelocityChange);
        }

        protected virtual void ApplyGravity()
        {
            AddForce(
                force: Vector3.down * 40,
                magnitude: 60,
                durationMode: DurationMode.Continuous
            );
        }

        protected virtual void HandleCollision(Collision collision, bool preserveMagnitude = false)
        {
            if (contacts.TryGetValue(collision.collider, out CollisionData data))
            {
                data.Update(collision);
            }
            else
            {
                data = new CollisionData(collision);
                contacts[collision.collider] = data;
            }

            UpdateContactVelocity();

            if (collision.collider.gameObject.layer.ToLayer() == Layer.Launch) return;

            body.linearVelocity = localVelocity.Project(
                normal: data.Normal,
                preserveMagnitude: preserveMagnitude) + contactVelocity;
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision, true);
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            contacts.Remove(collision.collider);
            UpdateContactVelocity();
        }

        protected virtual void ApplyContacts()
        {
            Vector3 delta;
            Vector3 current;
            foreach (var (collider, data) in contacts)
            {
                if (data.Rigidbody == null)
                    continue;

                current = data.Rigidbody.GetPointVelocity(data.Point);
                delta = current - data.PointVelocity;

                data.PointVelocity = current;
                body.linearVelocity += delta;
            }
        }

        protected virtual void FixedUpdate()
        {
            ApplyContacts();
            UpdateContactVelocity();
            ApplyGravity();
        }
    }
}