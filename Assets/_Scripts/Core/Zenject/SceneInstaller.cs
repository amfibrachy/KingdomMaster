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
        
        [Header("General parent Objects")]
        [SerializeField] private Transform _particleParent;
        [SerializeField] private Transform _buildingsParent;
        [SerializeField] private Transform _wallsParent;
        [SerializeField] private Transform _constructionSitesParent;
        
        [Header("NPC parent Objects")]
        [SerializeField] private Transform _sluggardsParent;
        [SerializeField] private Transform _buildersParent;
        
        [Header("Controllers and Managers")]
        [SerializeField] private SluggardsManager _sluggardsManager;
        [SerializeField] private BuildersManager _buildersManager;
        [SerializeField] private BuildingsManager _buildingsManager;
        [SerializeField] private PopulationController _populationController;
        [SerializeField] private UIUpdateController _uiUpdateController;
        [SerializeField] private PlacementSystemScript _placementSystem ;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_camera);
            Container.BindInstance(_sluggardsManager);
            Container.BindInstance(_buildersManager);
            Container.BindInstance(_buildingsManager);
            Container.BindInstance(_populationController);
            Container.BindInstance(_uiUpdateController);
            Container.BindInstance(_placementSystem);
            
            Container.BindInstance(_particleParent).WithId("ParticleParent");
            Container.BindInstance(_buildingsParent).WithId("BuildingsParent");
            Container.BindInstance(_wallsParent).WithId("WallsParent");
            Container.BindInstance(_constructionSitesParent).WithId("ConstructionSitesParent");

            Container.BindInstance(_sluggardsParent).WithId("SluggardsParent");
            Container.BindInstance(_buildersParent).WithId("BuildersParent");
        }
    }
}
