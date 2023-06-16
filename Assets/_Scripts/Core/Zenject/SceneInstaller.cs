namespace _Scripts.Core.Zenject
{
    using global::Zenject;
    using UnityEngine;

    public class SceneInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField] private Camera _camera;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_camera);
        }
    }
}
