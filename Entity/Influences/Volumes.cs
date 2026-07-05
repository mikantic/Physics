using System.Linq;
using Core.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    [RequireComponent(typeof(Body))]
    public class Volumes : Influencee<Volume>
    {
        [SerializeField] private Body _body;

        public float Density { get; private set; }

        #if UNITY_EDITOR
        private void Reset()
        {
            TryGetComponent(out _body);
        }
        #endif

        public Vector3 GetFlowForce(out float magnitude)
        {
            Vector3 force = Influencers.Data.Values
                .Select(contact => contact.Flow(_body.transform.position))
                .Average(emptyResult: Vector3.zero); 

            magnitude = force.magnitude;
            return force;
        }

        public Vector3 GetResistanceForce(out float magnitude)
        {
            float scaler = Influencers.Data.Values
                .Select(influence => influence.Density)
                .Maximum(emptyResult: 0);

            Vector3 force = -_body.LocalVelocity * scaler * 0.5f;
            magnitude = force.magnitude;
            return force.normalized * Time.fixedDeltaTime;
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

        protected void UpdateDensity()
        {
            Density = Influencers.Data.Values.Select(influence => influence.Density).Maximum(emptyResult: 0);
        }

        protected override void RemoveInfluence(Collider collider)
        {
            base.RemoveInfluence(collider);
            UpdateDensity();
        }

        protected override void UpdateInfluence(Collider collider, Volume influencer)
        {
            base.UpdateInfluence(collider, influencer);
            UpdateDensity();
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