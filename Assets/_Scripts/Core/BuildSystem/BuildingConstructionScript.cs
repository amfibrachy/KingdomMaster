namespace _Scripts.Core.BuildSystem
{
    using TMPro;
    using UnityEngine;

    public class BuildingConstructionScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _progressText;

        private float _currentProgress;
        private float _buildTime;
        private float _buildingWidth;
        private float _buildersNeeded;
        private BuildingPlacementScript _buildingPrefab;

        public float BuildingWidth => _buildingWidth;
        public float BuildersNeeded => _buildersNeeded;

        public int NumberOfBuildersBusy { get; set; } = 0;

        public bool IsConstructionCanceled { get; private set; }
        public bool IsConstructionFinished => _currentProgress >= _buildTime * _buildersNeeded;
        
        public void InitConstructionSite(BuildingDataSO data)
        {
            _buildTime = data.BuildTime;
            _buildingPrefab = data.Prefab;
            _buildingWidth = data.BuildingWidth;
            _buildersNeeded = data.BuildersNeeded;

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
