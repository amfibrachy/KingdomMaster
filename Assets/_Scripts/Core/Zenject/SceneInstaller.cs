namespace _Scripts.Core.Zenject
{
    using BuildSystem;
    using BuildSystem.TownCenter;
    using Global;
    using global::Zenject;
    using JobSystem;
    using NPC;
    using UnityEngine;

    public class SceneInstaller : MonoInstaller<ProjectContextInstaller>
    {
        [Header("Common Objects")]
        [SerializeField] private Camera _camera;
        [SerializeField] private TownCenterScript _townCenter;
        
        [Header("General parent Objects")]
        [SerializeField] private Transform _particleParent;
        [SerializeField] private Transform _buildingsParent;
        [SerializeField] private Transform _wallsParent;
        [SerializeField] private Transform _constructionSitesParent;
        
        [Header("NPC parent Objects")]
        [SerializeField] private Transform _sluggardsParent;
        [SerializeField] private Transform _buildersParent;
        [SerializeField] private Transform _lumberjacksParent;
        
        [Header("Controllers and Managers")]
        [SerializeField] private SluggardsManager _sluggardsManager;
        [SerializeField] private BuildersManager _buildersManager;
        [SerializeField] private LumberjacksManager _lumberjacksManager;
        [SerializeField] private BuildingsManager _buildingsManager;
        [SerializeField] private PopulationController _populationController;
        [SerializeField] private UIUpdateController _uiUpdateController;
        [SerializeField] private PlacementSystemScript _placementSystem;
        [SerializeField] private KingdomBordersController _kingdomBordersController;
        
        public override void InstallBindings()
        {
            // Common Objects
            Container.BindInstance(_camera);
            Container.BindInstance(_townCenter);
            
            // Controllers and Managers
            Container.BindInstance(_sluggardsManager);
            Container.BindInstance(_buildersManager);
            Container.BindInstance(_lumberjacksManager);
            Container.BindInstance(_buildingsManager);
            Container.BindInstance(_populationController);
            Container.BindInstance(_uiUpdateController);
            Container.BindInstance(_placementSystem);
            Container.BindInstance(_kingdomBordersController);
            
            // General parent Objects
            Container.BindInstance(_particleParent).WithId("ParticleParent");
            Container.BindInstance(_buildingsParent).WithId("BuildingsParent");
            Container.BindInstance(_wallsParent).WithId("WallsParent");
            Container.BindInstance(_constructionSitesParent).WithId("ConstructionSitesParent");

            // NPC parent Objects
            Container.BindInstance(_sluggardsParent).WithId("SluggardsParent");
            Container.BindInstance(_buildersParent).WithId("BuildersParent");
            Container.BindInstance(_lumberjacksParent).WithId("LumberjacksParent");
        }
    }
}
