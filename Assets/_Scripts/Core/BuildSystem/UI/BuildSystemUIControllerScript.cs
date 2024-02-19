namespace _Scripts.Core.JobSystem.UI
{
    using System;
    using System.Collections;
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
        
        // Privates
        private bool IsBuildingButtonsPanelHovered { set; get; }
        private bool IsInfoPanelHovered { set; get; }
        private bool HoveredEntryOnce { set; get; }

        private Coroutine _hideCoroutine;
        
        // Injectables
        private PlacementSystemScript _placementSystem;

        [Inject]
        public void Construct(PlacementSystemScript placementSystem)
        {
            _placementSystem = placementSystem;
        }
        
        private void Awake()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered += BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked += BuildingEntryClicked;
            }

            _buildSystemBuildingButtonsPanel.OnBuildingPanelHovered += BuildingButtonsPanelHovered;
            _buildSystemBuildingButtonsPanel.OnBuildingPanelExit += BuildingButtonsPanelExit;
            _buildingSystemInfo.OnInfoPanelHovered += InfoPanelHovered;
            _buildingSystemInfo.OnInfoPanelExit += InfoPanelExit;
            
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered -= BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked -= BuildingEntryClicked;
            }
            
            _buildSystemBuildingButtonsPanel.OnBuildingPanelHovered -= BuildingButtonsPanelHovered;
            _buildSystemBuildingButtonsPanel.OnBuildingPanelExit -= BuildingButtonsPanelExit;
            _buildingSystemInfo.OnInfoPanelHovered -= BuildingButtonsPanelHovered;
            _buildingSystemInfo.OnInfoPanelExit -= BuildingButtonsPanelExit;
            
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

        private void BuildingButtonsPanelHovered()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
            }

            IsBuildingButtonsPanelHovered = true;
        }

        private void BuildingButtonsPanelExit()
        {
            IsBuildingButtonsPanelHovered = false;
            _hideCoroutine = StartCoroutine(HideInfoPanelWithDelay());
        }
        
        private IEnumerator HideInfoPanelWithDelay()
        {
            yield return new WaitForSeconds(0.25f);

            if (!IsInfoPanelHovered && !IsBuildingButtonsPanelHovered)
            {
                HoveredEntryOnce = false;
                _buildingSystemInfo.HidePanel();
            }
        }

        private void InfoPanelHovered()
        {
            IsInfoPanelHovered = true;
        }
        
        private void InfoPanelExit()
        {
            IsInfoPanelHovered = false;
            _hideCoroutine = StartCoroutine(HideInfoPanelWithDelay());
        }
        
        private void BuildingEntryHovered(BuildingDataSO data)
        {
            if (IsBuildingButtonsPanelHovered && !HoveredEntryOnce)
            {
                HoveredEntryOnce = true;
                _buildingSystemInfo.ShowPanel();
            }

            _buildingSystemInfo.UpdateInfo(data);
        }

        private void BuildingEntryClicked(BuildingDataSO data)
        {
            _placementSystem.StartPlacement(data);

            BuildingButtonsPanelExit();
            HidePanel();
        }
        
        private void OnExitClicked()
        {
            HidePanel();
        }
    }
}
