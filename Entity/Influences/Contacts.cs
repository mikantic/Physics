using System.Collections.Generic;
using System.Linq;
using Core.Numerics;
using Core.Tools;
using UnityEngine;

namespace Physics.Influences
{
    public class Contacts : Influencee<Contact>
    {
        [SerializeField] protected LayerMask _layers;

        public Observable<Vector3> Normal { get; } = new(-UnityEngine.Physics.gravity.normalized);

        private void RecalculateNormal()
        {
            if (Influencers.Count <= 0) 
            {
                Normal.Value = -UnityEngine.Physics.gravity.normalized;
                return;
            }

            Normal.Value = Influencers.Data.Values.Select(contact => contact.Normal).Average(
                emptyResult: -UnityEngine.Physics.gravity.normalized);
        }

        protected override void UpdateInfluence(Collider collider, Contact influencer)
        {
            base.UpdateInfluence(collider, influencer);
            RecalculateNormal();
        }

        protected override void RemoveInfluence(Collider collider)
        {
            base.RemoveInfluence(collider);
            RecalculateNormal();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.gameObject.layer.InLayerMask(_layers)) return;
            UpdateInfluence(collision.collider, new Contact(collision));
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!collision.collider.gameObject.layer.InLayerMask(_layers)) return;
            UpdateInfluence(collision.collider, new Contact(collision));
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!collision.collider.gameObject.layer.InLayerMask(_layers)) return;
            RemoveInfluence(collision.collider);
        }

        public void Clear()
        {
            IEnumerable<Collider> colliders = Influencers.Data.Keys.ToList();
            foreach (Collider collider in colliders) RemoveInfluence(collider);
        }
    }
}