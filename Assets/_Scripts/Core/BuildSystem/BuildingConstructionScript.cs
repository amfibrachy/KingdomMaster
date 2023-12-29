namespace _Scripts.Core.BuildSystem
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(BuildingPlacementScript))]
    public class BuildingConstructionScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _progressText;

        private float _currentProgress;
        private float _buildTime;
        private float _buildingWidth;
        private float _buildersNeeded;
        private BuildingPlacementScript _buildingPrefab;
        private BuildingPlacementScript _constructionPlacement;

        public float BuildingWidth => _buildingWidth;
        public float BuildersNeeded => _buildersNeeded;
        
        public bool IsConstructionCanceled { get; private set; }
        public bool IsConstructionFinished => _currentProgress >= _buildTime * _buildersNeeded;

        private void Awake()
        {
            _constructionPlacement = GetComponent<BuildingPlacementScript>();
        }

        public void InitConstructionSite(BuildingDataSO data)
        {
            _buildTime = data.BuildTime;
            _buildingWidth = data.BuildingWidth;
            _buildingPrefab = data.Prefab;
            _buildersNeeded = data.BuildersNeeded;
            
            _constructionPlacement.Initialize(data);
            
            _currentProgress = 0;
            UpdateProgressText(0);
        }

        public BuildingPlacementScript GetBuildingPrefab()
        {
            return _buildingPrefab;
        }

        private void UpdateProgressText(int amount)
        {
            if (amount > 100)
                amount = 100;
            
            _progressText.SetText(amount + "%");
        }

        public void AddProgress(float amount)
        {
            _currentProgress += amount;
            UpdateProgressText((int) (_currentProgress / (_buildTime * _buildersNeeded) * 100));
        }

        public void CancelConstruction()
        {
            IsConstructionCanceled = true;
        }
    }
}
