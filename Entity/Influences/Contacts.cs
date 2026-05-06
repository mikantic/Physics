using System.Linq;
using Core.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public class Contacts : Influencee<Contact>
    {
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
                emptyResult: -UnityEngine.Physics.gravity.normalized);
        }

        private void OnCollisionEnter(Collision collision)
        {
            UpdateInfluence(collision.collider, new Contact(collision));
        }

        private void OnCollisionStay(Collision collision)
        {
            UpdateInfluence(collision.collider, new Contact(collision));
        }

        private void OnCollisionExit(Collision collision)
        {
            RemoveInfluence(collision.collider);
        }
    }
}