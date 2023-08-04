namespace _Scripts.Core.BuildSystem
{
    using TMPro;
    using UnityEngine;

    public class BuildingConstructionScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _progressText;

        private float _currentProgress;
        private float _buildTime;
        private BuildingPlacementScript _buildingPrefab;

        public void SetData(BuildingDataSO data)
        {
            _buildTime = data.BuildTime;
            _buildingPrefab = data.Prefab;
            
            UpdateProgressText(0);
        }

        public BuildingPlacementScript GetBuildingPrefab()
        {
            return _buildingPrefab;
        }

        public void UpdateProgressText(float amount)
        {
            _progressText.SetText(amount + "%");
        }

        public bool IsConstructionFinished()
        {
            return _currentProgress >= _buildTime;
        }
        
        public void AddProgress(float amount)
        {
            _currentProgress += amount;
            UpdateProgressText(_currentProgress);
        }
    }
}
