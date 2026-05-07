using UnityEngine;

namespace Physics.Influences
{
    public interface IVolumeFlow
    {
        public Vector3 Flow(Vector3 position);
        public float Density { get; }
    }
}