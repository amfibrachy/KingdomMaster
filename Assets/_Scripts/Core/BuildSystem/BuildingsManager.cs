namespace _Scripts.Core.BuildSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using Global;
    using global::Zenject;
    using JobSystem;
    using NPC;
    using UnityEngine;
    using Utils;

    public class BuildingsManager : MonoBehaviour
    {
        [SerializeField] private LayerMask _buildingLayer;
        [SerializeField] private Transform _buildingsSearchOrigin;
        [SerializeField] private float _raycastDistance;
        [SerializeField] private int _maxBuildingToRaycast;
        
        // Injectables
        [Inject(Id = "BuildingsParent")] private Transform _buildingsParent;
        private KingdomBordersController _kingdomBordersController;
        private LumberjacksManager _lumberjacksManager;
        
        [Inject]
        public void Construct(
            KingdomBordersController kingdomBordersController,
            LumberjacksManager lumberjacksManager)
        {
            _kingdomBordersController = kingdomBordersController;
            _lumberjacksManager = lumberjacksManager;
        }
        
        private void Start()
        {
            var initialBuildings = Util.GetActiveChildComponents<BuildingDataScript>(_buildingsParent);

            foreach (var building in initialBuildings)
            {
                AddConstructedBuilding(building);
            }
        }
        
        public void AddConstructedBuilding(BuildingDataScript building)
        {
            // Initialize data if is Job building
            var buildingJob = building.GetComponent<BuildingJobScript>();

            if (buildingJob != null) // TODO Can be changed later to initialize all needed component scripts
            {
                buildingJob.Initialize(building.Data);
            }

            switch (building.Type)
            {
                case BuildingType.TownCenter:
                    break;
                case BuildingType.ArcherTower:
                    break;
                case BuildingType.VillagerHouse:
                    break;
                case BuildingType.LumberjacksHut:
                    // Create lumberjack Hut tree reach map
                    _lumberjacksManager.AddLumberjackHut(building);
                    
                    break;
                case BuildingType.Wall:
                    // Update kingdom border controller if it is wall
                    _kingdomBordersController.AddWall(building);
                    
                    break;
                case BuildingType.Blacksmith:
                    break;
                case BuildingType.Windmill:
                    break;
                case BuildingType.MinersShaft:
                    break;
                case BuildingType.FishingDock:
                    break;
                case BuildingType.Eatery:
                    break;
                case BuildingType.EngineersCabin:
                    break;
                case BuildingType.MageTower:
                    break;
                case BuildingType.AlchemistHouse:
                    break;
                case BuildingType.Stockpile:
                    break;
                case BuildingType.HerbalistShack:
                    break;
            }

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
