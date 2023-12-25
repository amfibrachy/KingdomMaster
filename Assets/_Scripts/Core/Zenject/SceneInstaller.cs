namespace _Scripts.Core.Zenject
{
    using global::Zenject;
    using UnityEngine;

    public class SceneInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _particleParent;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_camera);
            Container.BindInstance(_particleParent).WithId("ParticleParent");
        }
    }
}
