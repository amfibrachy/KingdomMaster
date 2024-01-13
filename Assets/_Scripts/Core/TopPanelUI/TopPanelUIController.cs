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
        
        
        private void Awake()
        {
            _jobsButton.onClick.AddListener(ToggleJobsPanel);
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
    }
}
