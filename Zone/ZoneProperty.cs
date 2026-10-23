using UnityEngine;

namespace Physics
{
    public class ZoneProperty : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] protected float resistance = 0f;
        public float Resistance { get => resistance; }

        public virtual Vector3 Flow { get => Vector3.zero; }
    }
}