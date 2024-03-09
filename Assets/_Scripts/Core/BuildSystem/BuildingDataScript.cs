namespace _Scripts.Core.BuildSystem
{
    using UnityEngine;
    
    public class BuildingDataScript : MonoBehaviour
    {
        [SerializeField] private BuildingDataSO _defaultData;
        
        public BuildingType Type => _data.Type;
        public BuildingDataSO Data => _data;

        private BuildingDataSO _data;

        private void Awake()
        {
            _data = _defaultData;
        }

        public void Init(BuildingDataSO data)
        {
            _data = data;
        }
    }
}
