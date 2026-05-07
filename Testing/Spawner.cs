using UnityEngine;

namespace Physics
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Body _prefab;
        [SerializeField] private Transform _inactive;

        [SerializeField] private float _duration = 10f;
        [SerializeField] private float _spacing = 3f;

        protected ObjectPool<Body> _pool;

        private float _lastSpawnTime;

        private void Awake()
        {
            _pool = new ObjectPool<Body>(
                prefab: _prefab,
                count: 25,
                maxCount: 100,
                activeParent: null,
                inactiveParent: _inactive
            );
        }

        public void Spawn(float duration = 10)
        {
            _pool.Return(_pool.Get(transform.position, Quaternion.identity), this, duration);
            _lastSpawnTime = Time.time;
        }

        public void Spawn(Vector3 velocity, float duration = 10)
        {
            Body body = _pool.Get(transform.position, Quaternion.identity);
            body.AddForce(velocity, velocity.magnitude, ForceType.Global);
            _pool.Return(body, this, duration);
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