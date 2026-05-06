using System.Linq;
using Core.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public class Volumes : Influencee<Volume>
    {
        [SerializeField] private Body _body;

        public Vector3 GetFlowForce(out float magnitude)
        {
            Vector3 force = Influencers.Data.Values
                .Select(contact => contact.Flow)
                .Average(emptyResult: Vector3.zero); 

            magnitude = force.magnitude;
            return force;
        }

        public Vector3 GetBuoyantForce(out float magnitude)
        {
            Vector3 force = Influencers.Data
                .Select(kvp => kvp.Value.GetBuoyantForce(kvp.Key, out float magnitude) * magnitude)
                .Average(emptyResult: Vector3.zero);

            magnitude = force.magnitude;
            return force;
        }

        public void OnTriggerEnter(Collider collider)
        {
            Volume volume = new Volume(collider, transform.position);
            UpdateInfluence(collider, volume);
            _body.AddForce(volume.GetTensionForce(_body.GlobalVelocity, out float magnitude), magnitude, ForceType.Global);
        }

        public void OnTriggerStay(Collider collider)
        {
            UpdateInfluence(collider, new Volume(collider, transform.position));
        }

        public void OnTriggerExit(Collider collider)
        {
            RemoveInfluence(collider);
        }
    }
}