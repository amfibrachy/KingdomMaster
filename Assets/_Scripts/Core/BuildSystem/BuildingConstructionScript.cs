namespace _Scripts.Core.BuildSystem
{
    using System;
    using TMPro;
    using UnityEngine;
    
    [RequireComponent(typeof(BuildingDataScript))]
    public class BuildingConstructionScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _progressText;

        private BuildingType _type;
        private float _currentProgress;
        private float _buildTime;
        private float _buildingWidth;
        private float _buildersNeeded;
        private BuildingDataScript _buildingPrefab;
        private BuildingDataScript _buildingDataScript;

        public event Action<BuildingConstructionScript, bool> OnConstructionFinished;
        
        public BuildingType Type => _type;
        public BuildingDataSO Data => _data;
        public float BuildingWidth => _buildingWidth;
        public float BuildersNeeded => _buildersNeeded;
        
        public bool IsConstructionCanceled { get; private set; }
        public bool IsConstructionFinished { get; private set; }

        private BuildingDataSO _data;

        private void Awake()
        {
            _buildingDataScript = GetComponent<BuildingDataScript>();
        }

        public void InitConstructionSite(BuildingDataSO data)
        {
            _data = data;
            _type = data.Type;
            _buildTime = data.BuildTime;
            _buildingWidth = data.BuildingWidth;
            _buildingPrefab = data.Prefab;
            _buildersNeeded = data.MaxBuildersAmount;
            
            _buildingDataScript.Init(data);

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

            if (_currentProgress >= _buildTime * _buildersNeeded)
            {
                IsConstructionFinished = true;
                OnConstructionFinished?.Invoke(this, false);
            }
        }

        public void CancelConstruction()
        {
            IsConstructionCanceled = true;
            OnConstructionFinished?.Invoke(this, true);
        }
    }
}
