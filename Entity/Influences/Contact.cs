using System.Linq;
using Core.Numerics;
using Core.Tools;
using Physics.Numerics;
using UnityEngine;

namespace Physics.Influences
{
    public class Contact : Influencer
    {
        private readonly Vector3 _normal;
        public Vector3 Normal => _normal;

        public Contact(Collision collision) : base(collision.Point(), collision.gameObject)
        {
            _normal = collision.Normal().normalized;
        }
    }
}