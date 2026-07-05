using System.Collections.Generic;
using System.Linq;
using Core.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public class Contacts : Influencee<Contact>
    {
        [SerializeField] protected LayerMask _layers;
        private Vector3 _normal = Vector3.up;
        public Vector3 Normal
        {
            get => _normal;
        }

        private void RecalculateNormal()
        {
            if (Influencers.Count <= 0) 
            {
                _normal = UnityEngine.Physics.gravity.normalized;
                return;
            }

            _normal = Influencers.Data.Values.Select(contact => contact.Normal).Average(
                emptyResult: UnityEngine.Physics.gravity.normalized);
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
            List<Collider> colliders = Influencers.Data.Keys.ToList();
            colliders.ForEach(collider => RemoveInfluence(collider));
        }
    }
}