namespace _Scripts.Core.BuildSystem
{
    using global::Zenject;
    using UnityEngine;

    public class BuildSystemController : MonoBehaviour
    {
        [SerializeField] private PlacementSystemScript _placementSystem;
        
        // Injectables
        private BuildersManager _buildersManager;
        private BuildingsManager _buildingsManager;
        
        [Inject(Id = "BuildingsParent")] private Transform _buildingsParent;

        [Inject]
        public void Construct(BuildersManager buildersManager, BuildingsManager buildingsManager)
        {
            _buildingsManager = buildingsManager;
            _buildersManager = buildersManager;
        }
        
        private void Awake()
        {
            _placementSystem.OnBuildingPlaced += PlaceConstructionSiteAndEnqueueRequest;
            _buildersManager.OnConstructionFinished += PlaceConstructedBuilding;
        }
        
        private void PlaceConstructionSiteAndEnqueueRequest(BuildingDataSO building, Vector2 position)
        {
            var constructionSite = Instantiate(building.ConstructionPrefab, position, Quaternion.identity, _buildingsParent);
            constructionSite.InitConstructionSite(building);
            
            _buildersManager.AddConstructionTask(constructionSite);
        }

        private void PlaceConstructedBuilding(BuildingConstructionScript constructionSite)
        {
            var constructionPosition = constructionSite.transform.position;

            var building = Instantiate(constructionSite.GetBuildingPrefab(), constructionPosition, Quaternion.identity, _buildingsParent);
            _buildingsManager.AddConstructedBuilding(building);
            
            Destroy(constructionSite.gameObject);
        }
    }
}
