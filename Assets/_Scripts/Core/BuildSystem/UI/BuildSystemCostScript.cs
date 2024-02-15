namespace _Scripts.Core.BuildSystem.UI
{
    using DG.Tweening;
    using UnityEngine;

    public class BuildSystemCostScript : MonoBehaviour
    {
        [Header("Cost panel")]
        [SerializeField] private CanvasGroup _buildingSystemCostCanvasGroup;
        [SerializeField] private RectTransform _costPanelTransform;
        [SerializeField] private float _initialPosX = 465f;
        [SerializeField] private float _targetPosX = 610f;
        
        public bool IsShown { private set; get; }
        public void ShowPanel()
        {
            _buildingSystemCostCanvasGroup.DOFade(1f, 0.15f);
            _costPanelTransform.DOAnchorPosX(_targetPosX, 0.3f).OnComplete(() =>
            {
                IsShown = true;
            });
        }

        public void HidePanel(bool instant = false)
        {
            _buildingSystemCostCanvasGroup.DOFade(0f, instant ? 0f : 0.15f);
            _costPanelTransform.DOAnchorPosX(_initialPosX, instant ? 0f : 0.3f).OnComplete(() =>
            {
                IsShown = false;
            });
        }
    }
}
