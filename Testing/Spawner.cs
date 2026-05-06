using UnityEngine;

namespace Physics
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Body _prefab;

        [SerializeField] private float _duration = 10f;
        [SerializeField] private float _spacing = 3f;

        private float _lastSpawnTime;

        public void Spawn(float duration = 10)
        {
            Destroy(Instantiate(_prefab, transform.position, Quaternion.identity).gameObject, duration);
            _lastSpawnTime = Time.time;
        }

        public void Spawn(Vector3 velocity, float duration = 10)
        {
            Body body = Instantiate(_prefab, transform.position, Quaternion.identity);
            body.AddForce(velocity, velocity.magnitude, ForceType.Global);
            Destroy(body.gameObject, duration);
            _lastSpawnTime = Time.time;
        }

        public void FixedUpdate()
        {
            if (Time.time - _lastSpawnTime > _spacing)
            {
                Spawn(_duration);
            }
        }
    }
   
}