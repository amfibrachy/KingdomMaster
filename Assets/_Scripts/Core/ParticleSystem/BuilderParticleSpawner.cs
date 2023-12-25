namespace _Scripts.Core.BuildSystem
{
    using global::Zenject;
    using NPC;
    using ParticleSystem;
    using UnityEngine;
    using UnityEngine.Pool;

    [RequireComponent(typeof(BuilderFSM))]
    public class BuilderParticleSpawner : MonoBehaviour
    {
        [SerializeField] private ParticlePool _particlePoolPrefab;
        
        // Injectables
        [Inject(Id = "ParticleParent")] private Transform _particleParent;
        
        public ObjectPool<ParticlePool> ParticlePool { get; private set; }
        private BuilderFSM _builder;

        void Start()
        {
            _builder = GetComponent<BuilderFSM>();
            
            ParticlePool = new ObjectPool<ParticlePool>(OnCreateParticles, OnTakeParticles, OnReturnParticles,
                OnDestroyParticles, true, 10, 100);
        }

        private ParticlePool OnCreateParticles()
        {
            ParticlePool particles = Instantiate(_particlePoolPrefab, _builder.BuildParticlesPosition, _particlePoolPrefab.transform.rotation, _particleParent);
            particles.SetPool(ParticlePool);
             
            return particles;
        }

        private void OnTakeParticles(ParticlePool particles)
        {
            particles.transform.position = _builder.BuildParticlesPosition;
            particles.transform.localScale = new Vector3(_builder.AnimationController.IsFacingRight ? 1 : -1, 1, 1);
            
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
