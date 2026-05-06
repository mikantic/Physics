using UnityEngine;

namespace Physics.Influences
{
    public class VolumeFlow : MonoBehaviour
    {
        [SerializeField] private Vector3 _flow;
        public Vector3 Flow { get => _flow; }

        [Range(0, 1)]
        [SerializeField] private float _density;
        public float Density { get => _density; }
    }
}