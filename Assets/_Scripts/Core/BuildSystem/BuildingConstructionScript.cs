namespace _Scripts.Core.BuildSystem
{
    using TMPro;
    using UnityEngine;
    
    public class BuildingConstructionScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _progressText;

        private BuildingType _type;
        private float _currentProgress;
        private float _buildTime;
        private float _buildingWidth;
        private float _buildersNeeded;
        private BuildingDataScript _buildingPrefab;

        public BuildingType Type => _type;
        public BuildingDataSO Data => _data;
        public float BuildingWidth => _buildingWidth;
        public float BuildersNeeded => _buildersNeeded;
        
        public bool IsConstructionCanceled { get; private set; }
        public bool IsConstructionFinished => _currentProgress >= _buildTime * _buildersNeeded;

        private BuildingDataSO _data;
        
        public void InitConstructionSite(BuildingDataSO data)
        {
            _data = data;
            _type = data.Type;
            _buildTime = data.BuildTime;
            _buildingWidth = data.BuildingWidth;
            _buildingPrefab = data.Prefab;
            _buildersNeeded = data.MaxBuildersAmount;

            _currentProgress = 0;
            UpdateProgressText(0);
        }

        public BuildingDataScript GetBuildingPrefab()
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
