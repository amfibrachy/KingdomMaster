namespace _Scripts.Core.BuildSystem.UI
{
    using DG.Tweening;
    using UnityEngine;

    public class BuildSystemInfoScript : MonoBehaviour
    {
        [Header("Info panel")]
        [SerializeField] private CanvasGroup _buildingSystemInfoCanvasGroup;
        [SerializeField] private RectTransform _infoPanelTransform;
        [SerializeField] private float _initialPosY = -841;
        
        [Header("Cost panel")] 
        [SerializeField] private BuildSystemCostScript _buildSystemCost;
        
        public bool IsShown { private set; get; }
        
        public void ShowPanel()
        {
            _buildingSystemInfoCanvasGroup.DOFade(1f, 0.15f);
            _infoPanelTransform.DOAnchorPosY(0f, 0.3f).OnComplete(() =>
            {
                IsShown = true;
                _buildSystemCost.ShowPanel();
            });
        }

        public void HidePanel()
        {
            _buildingSystemInfoCanvasGroup.DOFade(0f, 0.15f);
            _infoPanelTransform.DOAnchorPosY(_initialPosY, 0.3f).OnComplete(() =>
            {
                IsShown = false;
                _buildSystemCost.HidePanel();
            });
        }

        public void UpdateInfo(BuildingDataSO data)
        {
            Debug.Log("Updated info");
        }
    }
}
