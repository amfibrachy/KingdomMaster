namespace _Scripts.Core.BuildSystem
{
    using global::Zenject;
    using UnityEngine;

    public class BuildSystemController : MonoBehaviour
    {
        // Injectables
        private BuildersManager _buildersManager;
        private BuildingsManager _buildingsManager;
        private PlacementSystemScript _placementSystem;
        
        [Inject(Id = "BuildingsParent")] private Transform _buildingsParent;
        [Inject(Id = "WallsParent")] private Transform _wallsParent;
        [Inject(Id = "ConstructionSitesParent")] private Transform _constructionSitesParent;

        [Inject]
        public void Construct(
            BuildersManager buildersManager, 
            BuildingsManager buildingsManager,
            PlacementSystemScript placementSystem)
        {
            _buildingsManager = buildingsManager;
            _buildersManager = buildersManager;
            _placementSystem = placementSystem;
        }
        
        private void Awake()
        {
            _placementSystem.OnBuildingPlaced += PlaceConstructionSiteAndEnqueueRequest;
            _buildersManager.OnConstructionFinished += PlaceConstructedBuilding;
        }
        
        private void PlaceConstructionSiteAndEnqueueRequest(BuildingDataSO building, Vector2 position)
        {
            var constructionSite = Instantiate(building.ConstructionPrefab, position, Quaternion.identity, _constructionSitesParent);
            constructionSite.InitConstructionSite(building);
            
            _buildersManager.AddConstructionTask(constructionSite);
        }

        private void PlaceConstructedBuilding(BuildingConstructionScript constructionSite)
        {
            var constructionPosition = constructionSite.transform.position;

            var building = Instantiate(constructionSite.GetBuildingPrefab(), constructionPosition, Quaternion.identity, constructionSite.Type == BuildingType.Wall ? _wallsParent : _buildingsParent);
            _buildingsManager.AddConstructedBuilding(building);
            
            Destroy(constructionSite.gameObject);
        }
    }
}
