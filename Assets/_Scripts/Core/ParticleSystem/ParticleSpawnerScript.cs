namespace _Scripts.Core.ParticleSystem
{
    using global::Zenject;
    using UnityEngine;
    using UnityEngine.Pool;

    [RequireComponent(typeof(IParticleEmitter))]
    public class ParticleSpawnerScript : MonoBehaviour
    {
        [SerializeField] private ParticlePool _particlePoolPrefab;
        
        // Injectables
        [Inject(Id = "ParticleParent")] private Transform _particleParent;
        
        public ObjectPool<ParticlePool> ParticlePool { get; private set; }
        private IParticleEmitter _emitter;

        void Start()
        {
            _emitter = GetComponent<IParticleEmitter>();
            
            ParticlePool = new ObjectPool<ParticlePool>(OnCreateParticles, OnTakeParticles, OnReturnParticles,
                OnDestroyParticles, true, 10, 100);
        }

        private ParticlePool OnCreateParticles()
        {
            ParticlePool particles = Instantiate(_particlePoolPrefab, _emitter.ParticlePosition, _particlePoolPrefab.transform.rotation, _particleParent);
            particles.SetPool(ParticlePool);
             
            return particles;
        }

        private void OnTakeParticles(ParticlePool particles)
        {
            particles.transform.position = _emitter.ParticlePosition;
            particles.transform.localScale = new Vector3(_emitter.AnimationController.IsFacingRight ? 1 : -1, 1, 1);
            
            particles.gameObject.SetActive(true);
        }

        private void OnReturnParticles(ParticlePool particles)
        {
            particles.gameObject.SetActive(false);
        }

        private void OnDestroyParticles(ParticlePool particles)
        {
            Destroy(particles.gameObject);
        }
    }
}
