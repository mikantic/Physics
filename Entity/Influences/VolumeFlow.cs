using UnityEngine;

namespace Physics.Influences
{
    public class VolumeFlow : MonoBehaviour, IVolumeFlow
    {
        [SerializeField] private Vector3 _flow;
        public virtual Vector3 Flow(Vector3 position) => _flow;

        [Range(0, 1)]
        [SerializeField] private float _density;
        public float Density { get => _density; }
    }
}