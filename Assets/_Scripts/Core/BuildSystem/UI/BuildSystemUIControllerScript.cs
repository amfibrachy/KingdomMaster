namespace _Scripts.Core.JobSystem.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BuildSystem;
    using BuildSystem.UI;
    using DG.Tweening;
    using global::Zenject;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class BuildSystemUIControllerScript : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _buildingSystemCanvasGroup;
        [SerializeField] private RectTransform _buildingPanelTransform;
        [SerializeField] private float _outsidePosY = -221f;

        [SerializeField] private BuildingEntryScript[] _buildingEntries;
        
        public bool IsShown { private set; get; }

        [Inject]
        public void Construct()
        {

        }
        
        private void Awake()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered += BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked += BuildingEntryClicked;
            }
        }

        private void OnDestroy()
        {
            foreach (var buildingEntry in _buildingEntries)
            {
                buildingEntry.OnBuildingEntryHovered -= BuildingEntryHovered;
                buildingEntry.OnBuildingEntryClicked -= BuildingEntryClicked;
            }
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
            _buildingPanelTransform.DOAnchorPosY(_outsidePosY, 0.3f).OnComplete(() =>
            {
                IsShown = false;
            });
        }

        private void BuildingEntryClicked(BuildingDataSO data)
        {

        }
        
        private void BuildingEntryHovered(BuildingDataSO data)
        {

        }
    }
}
