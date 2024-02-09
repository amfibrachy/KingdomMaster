namespace _Scripts.Core.Zenject
{
    using BuildSystem;
    using Global;
    using global::Zenject;
    using JobSystem;
    using UnityEngine;

    public class SceneInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [SerializeField] private Camera _camera;
        
        [Header("Parent Objects")]
        [SerializeField] private Transform _particleParent;
        [SerializeField] private Transform _buildingsParent;
        
        [Header("Controllers and Managers")]
        [SerializeField] private SluggardsManager _sluggardsManager;
        [SerializeField] private BuildersManager _buildersManager;
        [SerializeField] private BuildingsManager _buildingsManager;
        [SerializeField] private PopulationController _populationController;
        [SerializeField] private UIUpdateController _uiUpdateController;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_camera);
            Container.BindInstance(_sluggardsManager);
            Container.BindInstance(_buildersManager);
            Container.BindInstance(_buildingsManager);
            Container.BindInstance(_populationController);
            Container.BindInstance(_uiUpdateController);
            
            Container.BindInstance(_particleParent).WithId("ParticleParent");
            Container.BindInstance(_buildingsParent).WithId("BuildingsParent");
        }
    }
}
