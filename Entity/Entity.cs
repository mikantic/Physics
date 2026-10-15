// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Core.Numerics;
// using Physics.Numerics;
// using UnityEngine;

// namespace Physics
// {
//     public enum ForceDuration
//     {
//         Instant, 
//         Continuous
//     }

//     public enum ForceReference
//     {
//         Global, 
//         Local
//     }

//     public enum ForceFriction
//     {
//         Against,
//         With,
//         None
//     }

//     public enum ForceSurfaces
//     {
//         All,
//         Valid,
//         None
//     }

//     public class Entity : MonoBehaviour
//     {
//         [SerializeField] protected Rigidbody _rigidbody;

//         [SerializeField] protected Force _gravity;

//         public virtual void ApplyForce(
//             Vector3 force,
//             float magnitude,
//             ForceDuration duration = ForceDuration.Instant,
//             ForceReference reference = ForceReference.Global,
//             ForceFriction friction = ForceFriction.Against,
//             ForceSurfaces surfaces = ForceSurfaces.All,
//             float nullifiedDot = -.707f,
//             float projectedDot = 0f,
//             bool preserveMagnitude = false
//         )
//         {
//             if (surfaces != ForceSurfaces.None)
//             {
//                 foreach (var kvp in surfaces switch
//                 {
//                     ForceSurfaces.Valid => _validSurfaces,
//                     ForceSurfaces.All or _ => _surfaces
//                 })
//                 {
//                     force = force.Project(
//                         normal: kvp.Value.Normal,
//                         nullifiedDot: nullifiedDot,
//                         projectedDot: projectedDot,
//                         preserveMagnitude: preserveMagnitude
//                     );

//                     // Vector3 localForce = kvp.Key.LocalVelocity(force, kvp.Value);
//                     // Vector3 frictionForce = kvp.Key.GetFriction(localForce, kvp.Value, friction);
//                     // force += frictionForce;
//                 }
//             }

//             if (friction != ForceFriction.None)
//             {
//                 float maxFriction = _validSurfaces.Keys.Select(surface => surface.Friction).Maximum(emptyResult: 0);
//                 force *= friction switch
//                 {
//                     ForceFriction.With => maxFriction,
//                     ForceFriction.Against => 1f - maxFriction,
//                     _ => 1f
//                 };
//             }

//             if (duration == ForceDuration.Continuous) force *= Time.fixedDeltaTime;
            
//             force = force.Clamp(magnitude, reference switch
//             {
//                 ForceReference.Local => _rigidbody.linearVelocity - _contactVelocity,
//                 ForceReference.Global or _=> _rigidbody.linearVelocity,
//             });

//             _rigidbody.AddForce(force, ForceMode.VelocityChange);
//         }

//         protected virtual void FixedUpdate()
//         {
//             ApplyContacts();
//             ApplyGravity();
//             ApplyFriction();

//             if ((_rigidbody.linearVelocity - _contactVelocity).magnitude < 0.01f)
//             {
//                 _rigidbody.linearVelocity = _contactVelocity;
//             }
//         }

//         #region Friction

//         /// <summary>
//         /// applies a stopping force locally
//         /// </summary>
//         protected virtual void ApplyFriction()
//         {
//             // ApplyForce(
//             //     force: -(_rigidbody.linearVelocity - _contactVelocity) * 4f, 
//             //     magnitude: 0,
//             //     duration: ForceDuration.Continuous,
//             //     reference: ForceReference.Local,
//             //     friction: ForceFriction.With,
//             //     surfaces: ForceSurfaces.Valid,
//             //     nullifiedDot: -1,
//             //     projectedDot: 1);
//         }

//         #endregion
//         #region Gravity

//         /// <summary>
//         /// applies global gravity force
//         /// </summary>
//         protected virtual void ApplyGravity()
//         {
//             ApplyForce(
//                 force: _gravity.Value,
//                 magnitude: _gravity.Magnitude,
//                 duration: ForceDuration.Continuous
//             );
//         }

//         #endregion
//         #region Surface

//         /// <summary>
//         /// surface contacts
//         /// </summary>
//         [SerializeField] protected Dictionary<Surface, Contact> _surfaces { get; } = new();

//         /// <summary>
//         /// valid surfaces
//         /// </summary>
//         protected IReadOnlyDictionary<Surface, Contact> _validSurfaces
//         {
//             get => _surfaces.Where(kvp => Valid(kvp.Key, kvp.Value)).ToDictionary(x => x.Key, x => x.Value);
//         }

//         /// <summary>
//         /// average contact velocity
//         /// </summary>
//         protected virtual Vector3 _contactVelocity
//         {
//             get => _validSurfaces.Values.OfType<DynamicContact>().Select(contact => contact.Velocity).Average(emptyResult: Vector3.zero);
//         }

//         /// <summary>
//         /// if a surface and contact are valid
//         /// </summary>
//         /// <param name="value"></param>
//         /// <returns></returns>
//         /// <exception cref="NotImplementedException"></exception>
//         protected virtual bool Valid(Surface surface, Contact contact)
//         {
//             return contact.Normal.Dot(_gravity.Value) < -.707f;// - 0.2f * (1f - surface.Friction);
//         }

//         /// <summary>
//         /// applies any changes in velocity
//         /// </summary>
//         protected virtual void ApplyContacts()
//         {
//             foreach (var kvp in _validSurfaces)
//             {
//                 if (kvp.Value is DynamicContact contact
//                 && kvp.Key is DynamicSurface surface)
//                 {
//                     Vector3 previous = contact.Velocity;
//                     Vector3 current = surface.Rigidbody.GetPointVelocity(contact.Point);
//                     if (current == previous) continue;

//                     contact.Velocity = current;
//                     _rigidbody.linearVelocity += current - previous;   
//                 }

//                 // Vector3 localVelocity = kvp.Key.LocalVelocity(_rigidbody.linearVelocity, kvp.Value);
//                 // Vector3 friction = kvp.Key.GetFriction(localVelocity, kvp.Value, ForceFriction.Against);
//                 // _rigidbody.linearVelocity += friction * 3f * Time.fixedDeltaTime;
//             }
//         }

//         /// <summary>
//         /// add collision
//         /// </summary>
//         /// <param name="collision"></param>
//         protected virtual void OnCollisionEnter(Collision collision)
//         {
//             if (!collision.gameObject.TryGetComponent(out Surface surface)) return;
//             Contact contact = surface.GetContact(collision);
//             _surfaces[surface] = contact;

//             if (!Valid(surface, contact)) return;
        
//             _rigidbody.linearVelocity = (_rigidbody.linearVelocity - _contactVelocity).Project(
//                 normal: contact.Normal,
//                 preserveMagnitude: false) + _contactVelocity;

//         }

//         /// <summary>
//         /// update collision
//         /// </summary>
//         /// <param name="collision"></param>
//         protected virtual void OnCollisionStay(Collision collision)
//         {
//             if (!collision.gameObject.TryGetComponent(out Surface surface)) return;
//             Contact contact = surface.GetContact(collision);

//             if (_surfaces.TryGetValue(surface, out Contact exsiting)) exsiting.Copy(contact);
//             else _surfaces[surface] = contact;

//             if (!Valid(surface, contact)) return;

//             _rigidbody.linearVelocity = (_rigidbody.linearVelocity - _contactVelocity).Project(
//                 normal: contact.Normal,
//                 preserveMagnitude: true) + _contactVelocity;
//         }

//         /// <summary>
//         /// remove collision
//         /// </summary>
//         /// <param name="collision"></param>
//         protected virtual void OnCollisionExit(Collision collision)
//         {
//             if (!collision.gameObject.TryGetComponent(out Surface surface)) return;
//             _surfaces.Remove(surface);
//         }

//         #endregion
//         #region Volumes

//         protected List<Zone> _zones { get; } = new();

//         protected virtual void OnTriggerEnter(Collider collider)
//         {
//             if (!collider.gameObject.TryGetComponent(out Zone zone)) return;
//             _zones.Add(zone);

//             // apply resistance tension
//         }

//         protected virtual void OnTriggerExit(Collider collider)
//         {
//             if (!collider.gameObject.TryGetComponent(out Zone zone)) return;
//             _zones.Remove(zone);
//         }


//         #endregion
//     }
// }