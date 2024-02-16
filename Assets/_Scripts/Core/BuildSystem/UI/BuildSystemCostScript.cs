namespace _Scripts.Core.BuildSystem.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using global::Zenject;
    using ResourceSystem;
    using ResourceSystem.UI;
    using UnityEngine;
    using UnityEngine.Serialization;
    using Utils.Debugging;

    public class BuildSystemCostScript : MonoBehaviour
    {
        [Header("Cost panel")]
        [SerializeField] private CanvasGroup _buildingSystemCostCanvasGroup;
        [SerializeField] private RectTransform _costPanelTransform;
        [SerializeField] private float _initialPosX = 465f;
        [SerializeField] private float _targetPosX = 610f;

        [Space] 
        [SerializeField] private ResourceEntryScript[] _resourceEntries;

        public bool IsShown { private set; get; }
        
        // Injectables
        [Inject] private IDebug _debug;
        
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

        public void UpdateCost(List<ResourceCost> costs)
        {
            if (costs.Count > _resourceEntries.Length)
            {
                _debug.LogError($"Costs ({costs.Count}) length is larger than resource entry number ({_resourceEntries.Length})");
            }

            for (var index = 0; index < _resourceEntries.Length; index++)
            {
                var entry = _resourceEntries[index];
                
                if (index < costs.Count)
                {
                    var cost = costs[index];
                    entry.SetEnabled(true);
                    entry.Set(cost.Resource.Icon, cost.Amount.ToString());
                }
                else
                {
                    entry.SetEnabled(false);
                }
            }
        }
    }
}
