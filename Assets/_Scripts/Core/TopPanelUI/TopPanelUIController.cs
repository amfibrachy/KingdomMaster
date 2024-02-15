namespace _Scripts.Core.TopPanelUI
{
    using System;
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
        
        
        private void Awake()
        {
            _jobsButton.onClick.AddListener(ToggleJobsPanel);
            _buildButton.onClick.AddListener(ToggleJBuildPanel);
        }

        private void ToggleJobsPanel()
        {
            if (_jobsSystemUIController.IsShown)
            {
                _jobsSystemUIController.HidePanel();
            }
            else
            {
                _jobsSystemUIController.ShowPanel();
            }
        }
        
        private void ToggleJBuildPanel()
        {
            if (_buildSystemUIController.IsShown)
            {
                _buildSystemUIController.HidePanel();
            }
            else
            {
                _buildSystemUIController.ShowPanel();
            }
        }
    }
}
