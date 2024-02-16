namespace _Scripts.Core.JobSystem.UI
{
    using System;
    using System.Collections.Generic;
    using BuildSystem;
    using BuildSystem.UI;
    using DG.Tweening;
    using global::Zenject;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class BuildSystemUIControllerScript : MonoBehaviour
    {
        [Header("Buildings panel")] 
        [SerializeField] private BuildSystemBuildingButtonsPanelScript _buildSystemBuildingButtonsPanel;
        [SerializeField] private CanvasGroup _buildingSystemCanvasGroup;
        [SerializeField] private RectTransform _buildingPanelTransform;
        [SerializeField] private float _initialPosY = -221f;
        [SerializeField] private Button _exitButton;
        
        [SerializeField] private BuildingEntryScript[] _buildingEntries;
        
        [Header("Info panel")]
        [SerializeField] private BuildSystemInfoScript _buildingSystemInfo;

        public bool IsShown { private set; get; }
        private bool IsPanelHovered { set; get; }
        private bool HoveredEntryOnce { set; get; }

        private void Awake()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered += BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked += BuildingEntryClicked;
            }

            _buildSystemBuildingButtonsPanel.OnBuildingPanelHovered += BuildingPanelHovered;
            _buildSystemBuildingButtonsPanel.OnBuildingPanelExit += BuildingPanelExit;
            
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered -= BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked -= BuildingEntryClicked;
            }
            
            _buildSystemBuildingButtonsPanel.OnBuildingPanelHovered -= BuildingPanelHovered;
            _buildSystemBuildingButtonsPanel.OnBuildingPanelExit -= BuildingPanelExit;
            
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        public void ShowPanel()
        {
            _buildingSystemCanvasGroup.DOFade(1f, 0.15f);
            _buildingPanelTransform.DOAnchorPosY(0f, 0.3f).OnComplete(() =>
            {
                IsShown = true;
            });
        }

        public void HidePanel()
        {
            _buildingSystemCanvasGroup.DOFade(0f, 0.15f);
            _buildingPanelTransform.DOAnchorPosY(_initialPosY, 0.3f).OnComplete(() =>
            {
                IsShown = false;
            });
        }

        private void BuildingPanelHovered()
        {
            IsPanelHovered = true;
        }
        
        private void BuildingPanelExit()
        {
            IsPanelHovered = false;
            HoveredEntryOnce = false;
            _buildingSystemInfo.HidePanel();
        }

        private void BuildingEntryHovered(BuildingDataSO data)
        {
            if (IsPanelHovered && !HoveredEntryOnce)
            {
                HoveredEntryOnce = true;
                _buildingSystemInfo.ShowPanel();
            }

            _buildingSystemInfo.UpdateInfo(data);
        }

        private void BuildingEntryClicked(BuildingDataSO data)
        {

        }
        
        private void OnExitClicked()
        {
            HidePanel();
        }
    }
}
