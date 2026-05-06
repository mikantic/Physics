using System.Linq;
using Core.Numerics;
using Core.Tools;
using UnityEngine;

namespace Physics.Influences
{
    public abstract class Influencee<I> : MonoBehaviour where I : Influencer
    {
        public Map<Collider, I> Influencers { get; } = new();

        public Vector3 Velocity => Influencers.Data.Values.Select(contact => contact.Velocity).Sum();

        protected virtual void UpdateInfluence(Collider collider, I influencer)
        {
            Influencers[collider] = influencer;
        }

        protected virtual void RemoveInfluence(Collider collider)
        {
            Influencers.Remove(collider);
        }
    }
}