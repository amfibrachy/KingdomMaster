namespace _Scripts.Core.BuildSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JobSystem;
    using UnityEngine;

    public class BuildingsManager : MonoBehaviour
    {
        [SerializeField] private BuildingPlacementScript[] _initialBuildings;

        [SerializeField] private LayerMask _buildingLayer;
        [SerializeField] private Transform _buildingsSearchOrigin;
        [SerializeField] private float _raycastDistance;
        [SerializeField] private int _maxBuildingToRaycast;
        
        private void Start()
        {
            foreach (var building in _initialBuildings)
            {
                AddConstructedBuilding(building);
            }
        }
        
        public void AddConstructedBuilding(BuildingPlacementScript building)
        {
            // Initialize data if is Job building
            var buildingJob = building.GetComponent<BuildingJobScript>();

            if (buildingJob != null) // TODO Can be changed later to initialize all needed component scripts
            {
                buildingJob.Initialize(building.Data);
            }
            
            // Update wall manager if is wall
            
            
            // TODO  add building to all buildings for tracking  _allBuildings.Add(buildingJob);
        }
     
        public List<BuildingJobScript> GetFreeBuildings(JobType job)
        {
            // If builder return single camp
            if (job == JobType.Builder)
            {
                return new List<BuildingJobScript> {_buildingsSearchOrigin.GetComponent<BuildingJobScript>()};
            }
            
            var hitsRight = new RaycastHit2D[_maxBuildingToRaycast / 2];
            var hitsLeft = new RaycastHit2D[_maxBuildingToRaycast / 2];
            
            Physics2D.RaycastNonAlloc(_buildingsSearchOrigin.position, Vector2.right, hitsRight, _raycastDistance, _buildingLayer);
            Physics2D.RaycastNonAlloc(_buildingsSearchOrigin.position, Vector2.left, hitsLeft, _raycastDistance, _buildingLayer);

            var combinedHits = new List<RaycastHit2D>(hitsLeft);
            combinedHits.AddRange(hitsRight);

            // Sort combined hits by distance
            combinedHits = combinedHits.OrderBy(hit => hit.distance).ToList();

            return combinedHits
                .Where(hit => hit.collider != null)
                .Select(hit => hit.collider.GetComponent<BuildingJobScript>())
                .Where(building => building != null && building.Job == job)
                .ToList();
        }
    }
}
