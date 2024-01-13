namespace _Scripts.Core.BuildSystem.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class BuildButtonsControllerScript : MonoBehaviour
    {
        [SerializeField] private BuildButtonScript _buildButtonPrefab;
        [SerializeField] private PlacementSystemScript _placementSystem;
        [SerializeField] private List<BuildingDataSO> _buildingsDatabase;

        private void Awake()
        {
            for (var index = 0; index < _buildingsDatabase.Count; index++)
            {
                var buttonData = _buildingsDatabase[index];
                
                var button = Instantiate(_buildButtonPrefab, transform.position, Quaternion.identity, transform);
                button.transform.SetParent(transform);

                button.SetId(index);
                button.SetIcon(buttonData.IconSprite);
                button.SetOnClickCallback(OnBuildingButtonClicked);
            }
        }

        private void OnBuildingButtonClicked(int index)
        {
            var buildingData = _buildingsDatabase[index];
            
            // TODO Check for sufficient resources and money
            
            _placementSystem.StartPlacement(buildingData);
        }
    }
}
