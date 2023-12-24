namespace _Scripts.Core.BuildSystem
{
    using UnityEngine;

    public class BuildSystemController : MonoBehaviour
    {
        [SerializeField] private PlacementSystemScript _placementSystem;
        [SerializeField] private Transform _buildingsParent;
        [SerializeField] private BuildersManager _buildersManager;

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

            Instantiate(constructionSite.GetBuildingPrefab(), constructionPosition, Quaternion.identity, _buildingsParent);
            
            Destroy(constructionSite.gameObject);
        }
    }
}
