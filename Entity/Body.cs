using System.Collections.Generic;
using Core.Numerics;
using Physics.Numerics;
using UnityEngine;

namespace Physics
{
    /// <summary>
    /// wrapper class for rigidbody to allow more control with forces and contacts
    /// </summary>
    public partial class Body : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// rigidbody
        /// </summary>
        [SerializeField] protected Rigidbody body;

        #endregion
        #region Properties

        /// <summary>
        /// contact collection
        /// </summary>
        protected Dictionary<Collider, Contact> contacts { get; } = new();

        /// <summary>
        /// max friction
        /// </summary>
        protected float? friction { get; set; } = null;

        /// <summary>
        /// average contact normal
        /// </summary>
        protected Vector3? normal { get; set; } = null;

        /// <summary>
        /// average contact normal for valid slopes
        /// </summary>
        protected Vector3? validNormal { get; set; } = null;

        /// <summary>
        /// average velocity of contacts
        /// </summary>
        protected Vector3? contactVelocity = null;

        /// <summary>
        /// zone collection
        /// </summary>
        protected Dictionary<Collider, Zone> zones { get; } = new();

        /// <summary>
        /// average velocity of zones
        /// </summary>
        protected Vector3? zoneVelocity = null;

        /// <summary>
        /// highest resistance of zones
        /// </summary>
        protected float? resistance { get; set; } = null;

        /// <summary>
        /// velocity local to contacts
        /// </summary>
        protected Vector3 localVelocity
        {
            get
            {
                return body.linearVelocity - (contactVelocity ?? Vector3.zero) - (zoneVelocity ?? Vector3.zero);
            }
        }

        #endregion
        #region Helper Methods

        /// <summary>
        /// if a vector is large enough to be valid
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        protected bool ValidStrength(Vector3 force) => force.sqrMagnitude > 0.0001f;

        /// <summary>
        /// applies delta velocit if any from data and updates given properties
        /// </summary>
        /// <param name="data"></param>
        /// <param name="collider"></param>
        /// <param name="velocity"></param>
        /// <param name="count"></param>
        protected void ApplyDeltaVelocity(Collider collider, Data data, ref Vector3? velocity, ref int count)
        {
            if (data.UpdateVelocity(collider.attachedRigidbody, out Vector3 delta))
            {
                body.linearVelocity += delta;
                velocity = (velocity ?? Vector3.zero) + data.Velocity;
                count++;
            }
        }

        /// <summary>
        /// applies contacts to force
        /// </summary>
        /// <param name="force"></param>
        /// <param name="dotBounds"></param>
        /// <param name="preserveMagnitude"></param>
        /// <returns></returns>
        protected bool TryApplyContacts(ref Vector3 force, DotBounds dotBounds, bool preserveMagnitude)
        {
            float dot;
            foreach(var (collider, contact) in contacts)
            {
                dot = force.Dot(contact.Normal);
                if (dot < dotBounds.Nullified) return false;
                if (dot < dotBounds.Projected) force = force.Project(contact.Normal, preserveMagnitude);
            }

            return ValidStrength(force);
        }

        /// <summary>
        /// applies friction mode to force
        /// </summary>
        /// <param name="force"></param>
        /// <param name="frictionMode"></param>
        /// <returns></returns>
        protected bool TryApplyFrictionMode(ref Vector3 force, FrictionMode frictionMode)
        {
            switch(frictionMode)
            {
                case FrictionMode.Against:
                    force *= 1f - (friction ?? 0f);
                    break;
                case FrictionMode.With:
                    force *= friction ?? 0f;
                    break;
            }

            return ValidStrength(force);
        }

        /// <summary>
        /// applies duration to force
        /// </summary>
        /// <param name="force"></param>
        /// <param name="durationMode"></param>
        /// <returns></returns>
        protected bool TryApplyDurationMode(ref Vector3 force, DurationMode durationMode)
        {
            switch(durationMode)
            {
                case DurationMode.Continuous:
                    force *= Time.fixedDeltaTime;
                    break;
            }

            return ValidStrength(force);
        }

        /// <summary>
        /// applies reference to force
        /// </summary>
        /// <param name="force"></param>
        /// <param name="magnitude"></param>
        /// <param name="referenceMode"></param>
        /// <returns></returns>
        protected bool TryApplyReferenceMode(ref Vector3 force, float magnitude, ReferenceMode referenceMode)
        {
            switch (referenceMode)
            {
                case ReferenceMode.Local:
                    force = force.Clamp(magnitude, localVelocity);
                    break;
                case ReferenceMode.Global:
                    force = force.Clamp(magnitude, body.linearVelocity);
                    break;
            }

            return ValidStrength(force);
        }

        /// <summary>
        /// applies zones to force and magntiude
        /// </summary>
        /// <param name="force"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        protected bool TryApplyZones(ref float magnitude)
        {
            if (resistance.HasValue) magnitude *= 1f - resistance.Value;
            return true;
        }

        #endregion
        #region Force Methods

        /// <summary>
        /// updates contact related values and applies changes in contact velocities
        /// </summary>
        protected virtual void ApplyContacts()
        {
            friction = null;
            normal = null;
            validNormal = null;
            int validCount = 0;
            contactVelocity = null;
            int velocityCount = 0;
            foreach (var (collider, contact) in contacts)
            {
                friction = Mathf.Max(friction ?? 0, contact.Friction);
                normal = (normal ?? Vector3.zero) + contact.Normal;
                if (contact.Normal.Dot(Vector3.down) < -0.7f)
                {
                    validNormal = (validNormal ?? Vector3.zero) + contact.Normal;
                    validCount++;  
                } 
                ApplyDeltaVelocity(collider, contact, ref contactVelocity, ref velocityCount);
            }
            if (contacts.Count > 0) normal /= contacts.Count;
            if (validCount > 0) validNormal /= validCount;
            if (velocityCount > 0) contactVelocity /= velocityCount;
        }

        /// <summary>
        /// applies global gravitational force
        /// </summary>
        protected virtual void ApplyGravity()
        {
            if (validNormal.HasValue) 
            {
                AddForce(-validNormal.Value, 1, DurationMode.Continuous, FrictionMode.None, ReferenceMode.Local, false);
                return;
            }
            AddForce(UnityEngine.Physics.gravity, DurationMode.Continuous, FrictionMode.Against, ReferenceMode.Global, false);
        }

        /// <summary>
        /// applies friction force to local velocity
        /// </summary>
        protected virtual void ApplyFriction()
        {
            if (!friction.HasValue) return;
            body.AddForce(-localVelocity * friction.Value, ForceMode.Acceleration);
        }

        /// <summary>
        /// applies zone related forces and values
        /// </summary>
        protected virtual void ApplyZones()
        {
            resistance = null;
            zoneVelocity = null;
            int velocityCount = 0;
            foreach(var (collider, zone) in zones)
            {
                resistance = Mathf.Max(resistance ?? 0, zone.Resistance);
                ApplyDeltaVelocity(collider, zone, ref zoneVelocity, ref velocityCount);
            }
            if (velocityCount > 0) zoneVelocity /= velocityCount;
        }

        /// <summary>
        /// adds a force with parameters
        /// </summary>
        /// <param name="force"></param>
        /// <param name="magnitude"></param>
        /// <param name="dotBounds"></param>
        /// <param name="durationMode"></param>
        /// <param name="frictionMode"></param>
        /// <param name="referenceMode"></param>
        /// <param name="preserveMagnitude"></param>
        public virtual void AddForce(
            Vector3 force,
            float magnitude,
            DotBounds dotBounds,
            DurationMode durationMode = DurationMode.Instant,
            FrictionMode frictionMode = FrictionMode.Against,
            ReferenceMode referenceMode = ReferenceMode.Global,
            bool preserveMagnitude = false
        )
        {
            if (!TryApplyContacts(ref force, dotBounds, preserveMagnitude)) return;
            if (!TryApplyZones(ref magnitude)) return;
            if (!TryApplyFrictionMode(ref force, frictionMode)) return;
            if (!TryApplyDurationMode(ref force, durationMode)) return;
            if (!TryApplyReferenceMode(ref force, magnitude, referenceMode)) return;
            body.AddForce(force, ForceMode.VelocityChange);
        }

        /// <summary>
        /// adds a force with default dot bounds
        /// </summary>
        /// <param name="force"></param>
        /// <param name="magnitude"></param>
        /// <param name="durationMode"></param>
        /// <param name="frictionMode"></param>
        /// <param name="referenceMode"></param>
        /// <param name="preserveMagnitude"></param>
        public virtual void AddForce(
            Vector3 force,
            float magnitude,
            DurationMode durationMode = DurationMode.Instant,
            FrictionMode frictionMode = FrictionMode.Against,
            ReferenceMode referenceMode = ReferenceMode.Global,
            bool preserveMagnitude = false
        )
        {
            AddForce(force, magnitude, DotBounds.Default, durationMode, frictionMode, referenceMode, preserveMagnitude);
        }

        /// <summary>
        /// add force assuming magnitude equals force magnitude
        /// </summary>
        /// <param name="force"></param>
        /// <param name="durationMode"></param>
        /// <param name="frictionMode"></param>
        /// <param name="referenceMode"></param>
        /// <param name="preserveMagnitude"></param>
        public virtual void AddForce(
            Vector3 force,
            DurationMode durationMode = DurationMode.Instant,
            FrictionMode frictionMode = FrictionMode.Against,
            ReferenceMode referenceMode = ReferenceMode.Global,
            bool preserveMagnitude = false
        )
        {
            AddForce(force, force.magnitude, DotBounds.Default, durationMode, frictionMode, referenceMode, preserveMagnitude);
        }

        #endregion
        #region Unity Methods

        /// <summary>
        /// applies contacts and common forces
        /// </summary>
        protected virtual void FixedUpdate()
        {
            ApplyContacts();
            ApplyZones();
            ApplyGravity();
            ApplyFriction();
            if (!ValidStrength(localVelocity)) 
            {
                body.linearVelocity = (contactVelocity ?? Vector3.zero) + (zoneVelocity ?? Vector3.zero);
            }
        }

        /// <summary>
        /// creates contact
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionEnter(Collision collision)
        {
            Contact contact = new(collision);
            contacts[collision.collider] = contact;
            body.linearVelocity = localVelocity.Project(contact.Normal, false) + contact.Velocity + (zoneVelocity ?? Vector3.zero);
        }

        /// <summary>
        /// updates contact
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionStay(Collision collision)
        {
            if (!contacts.TryGetValue(collision.collider, out Contact contact))
            {
                OnCollisionEnter(collision);
                return;
            }
            contact.Update(collision);
            body.linearVelocity = localVelocity.Project(contact.Normal, true) + contact.Velocity + (zoneVelocity ?? Vector3.zero);
        }

        /// <summary>
        /// removes contact
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionExit(Collision collision)
        {
            contacts.Remove(collision.collider);
        }

        /// <summary>
        /// adds zone
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerEnter(Collider collider)
        {
            Zone zone = new(collider);
            zones[collider] = zone;
            
            body.linearVelocity *= zone.Resistance;
        }

        /// <summary>
        /// updates zone
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OntriggerStay(Collider collider)
        {
            if (!zones.TryGetValue(collider, out Zone zone))
            {
                OnTriggerEnter(collider);
                return;
            }
            zone.Update(collider);
        }

        /// <summary>
        /// removes zone
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void OnTriggerExit(Collider collider)
        {
            zones.Remove(collider);
        }

        #endregion
    }
}