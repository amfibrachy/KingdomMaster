namespace _Scripts.Core.TopPanelUI
{
    using System;
    using BuildSystem;
    using global::Zenject;
    using JobSystem.UI;
    using UnityEngine;
    using UnityEngine.UI;

    public class TopPanelUIController : MonoBehaviour
    {
        [SerializeField] private Button _buildButton;
        [SerializeField] private Button _jobsButton;
        [SerializeField] private Button _armyButton;

        [SerializeField] private JobSystemUIControllerScript _jobsSystemUIController;
        [SerializeField] private BuildSystemUIControllerScript _buildSystemUIController;

        [Inject] private PlacementSystemScript _placementSystem;
        
        private void Awake()
        {
            _jobsButton.onClick.AddListener(ToggleJobsPanel);
            _buildButton.onClick.AddListener(ToggleJBuildPanel);
        }

        private void ToggleJobsPanel()
        {
            _placementSystem.StopPlacement();
            
            if (_jobsSystemUIController.IsShown)
            {
                _jobsSystemUIController.HidePanel();
            }
            else
            {
                _buildSystemUIController.HidePanel();
                _jobsSystemUIController.ShowPanel();
            }
        }
        
        private void ToggleJBuildPanel()
        {
            _placementSystem.StopPlacement();
            
            if (_buildSystemUIController.IsShown)
            {
                _buildSystemUIController.HidePanel();
            }
            else
            {
                _jobsSystemUIController.HidePanel();
                _buildSystemUIController.ShowPanel();
            }
        }
    }
}
