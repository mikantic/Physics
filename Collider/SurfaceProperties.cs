using UnityEngine;
namespace Physics
{
    public class SurfaceProperties : MonoBehaviour
    {
        [SerializeField] private float friction;
        public float Friction { get => friction; }

        [SerializeField] private float bounciness;
        public float Bounciness { get => bounciness; }
    }
}
