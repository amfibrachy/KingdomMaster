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
        
        // Privates
        private Sequence _sequence;
        
        public void ShowPanel()
        {
            // Cancel any ongoing sequence
            if (_sequence != null && _sequence.IsActive()) 
                _sequence.Kill();

            _sequence = DOTween.Sequence();

            _sequence.Append(_buildingSystemCostCanvasGroup.DOFade(1f, 0.15f))
                .Join(_costPanelTransform.DOAnchorPosX(_targetPosX, 0.3f))
                .AppendCallback(() =>
                {
                    IsShown = true;
                });
        }

        public void HidePanel(bool instant = false)
        {
            // Cancel any ongoing sequence
            if (_sequence != null && _sequence.IsActive()) 
                _sequence.Kill();

            _sequence = DOTween.Sequence();

            _sequence.Append(_buildingSystemCostCanvasGroup.DOFade(0f, instant ? 0f : 0.15f))
                .Join(_costPanelTransform.DOAnchorPosX(_initialPosX, instant ? 0f : 0.3f))
                .AppendCallback(() =>
                {
                    IsShown = false;
                });
        }
    }
}
