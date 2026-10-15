using UnityEngine;

namespace Physics
{
    public class Zone : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] protected float _resistance = 0f;
        public float Resistance => _resistance;

        [SerializeField] protected Force _force;
        public Force Force => _force;

        // to do, buoyancy, potentially in another class, as well as vortex which maybe will pull body toward center or something
    }
}