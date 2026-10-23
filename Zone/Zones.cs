using UnityEngine;

namespace Physics
{
    public class Zone : Data
    {
        public float Resistance { get; private set; }
        public virtual Vector3 Flow { get; protected set; }

        public Zone(Collider collider)
        {
            Update(collider);
            if (collider.TryGetComponent(out ZoneProperty property))
            {
                Resistance = property.Resistance;
                Flow = property.Flow;
            }
        }

        public void Update(Collider collider)
        {
            
        }
    }
}