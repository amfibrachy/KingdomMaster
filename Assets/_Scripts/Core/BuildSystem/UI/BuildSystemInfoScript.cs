namespace _Scripts.Core.BuildSystem.UI
{
    using System;
    using System.Collections;
    using DG.Tweening;
    using JobSystem;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class BuildSystemInfoScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Info panel")]
        [SerializeField] private CanvasGroup _buildingSystemInfoCanvasGroup;
        [SerializeField] private ContentSizeFitter _buildingSystemInfoFitter;
        [SerializeField] private RectTransform _infoPanelTransform;
        [SerializeField] private float _targetPosY = 840f;

        [Space]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _hp;

        [SerializeField] private RectTransform _residentsTransform;
        [SerializeField] private TextMeshProUGUI _residentsAmount;
        
        [SerializeField] private RectTransform _effectiveRangeTransform;
        [SerializeField] private TextMeshProUGUI _effectiveRangeAmount;

        [SerializeField] private RectTransform _jobDetails;
        
        [SerializeField] private Image _jobIcon;
        [SerializeField] private TextMeshProUGUI _jobName;
        [SerializeField] private TextMeshProUGUI _trainingCapacity;
        [SerializeField] private TextMeshProUGUI _trainingTime;
        
        [SerializeField] private TextMeshProUGUI _maxBuildersAmount;
        [SerializeField] private TextMeshProUGUI _buildTime;
        
        [Header("Cost panel")] 
        [SerializeField] private BuildSystemCostScript _buildSystemCost;
        
        public event Action OnInfoPanelHovered;
        public event Action OnInfoPanelExit;
        
        public bool IsShown { private set; get; }
        
        // Privates
        private Sequence _sequence;
        
        public void ShowPanel()
        {
            // Cancel any ongoing sequence
            if (_sequence != null && _sequence.IsActive()) 
                _sequence.Kill();

            _sequence = DOTween.Sequence();

            _sequence.Append(_buildingSystemInfoCanvasGroup.DOFade(1f, 0.15f))
                .Join(_infoPanelTransform.DOAnchorPosY(_targetPosY, 0.3f))
                .AppendCallback(() =>
                {
                    IsShown = true;
                    _buildSystemCost.ShowPanel();
                });
        }

        public void HidePanel()
        {
            // Cancel any ongoing sequence
            if (_sequence != null && _sequence.IsActive()) 
                _sequence.Kill();
            
            _buildSystemCost.HidePanel(true);

            _sequence = DOTween.Sequence();

            _sequence.Append(_buildingSystemInfoCanvasGroup.DOFade(0f, 0.15f))
                .Join(_infoPanelTransform.DOAnchorPosY(0, 0.3f))
                .AppendCallback(() =>
                {
                    IsShown = false;
                });
        }

        public void UpdateInfo(BuildingDataSO data)
        {
            _title.SetText(data.Name);
            _description.SetText(data.Description);
            _hp.SetText(data.HP.ToString());

            if (data.Capacity > 0)
            {
                _residentsTransform.gameObject.SetActive(true);
                _residentsAmount.SetText(data.Capacity.ToString());
            }
            else
            {
                _residentsTransform.gameObject.SetActive(false);
            }
            
            if (data.EffectiveRange > 0)
            {
                _effectiveRangeTransform.gameObject.SetActive(true);
                _effectiveRangeAmount.SetText(data.EffectiveRange.ToString());
            }
            else
            {
                _effectiveRangeTransform.gameObject.SetActive(false);
            }

            if (data.Job != JobType.None)
            {
                _jobDetails.gameObject.SetActive(true);
                _jobIcon.sprite = data.JobSprite;
                _jobName.SetText(data.Job.ToString());
                _trainingCapacity.SetText(data.TrainingCapacity.ToString());
                _trainingTime.SetText(data.TrainingTime.ToString());
            }
            else
            {
                _jobDetails.gameObject.SetActive(false);
            }
            
            _maxBuildersAmount.SetText(data.MaxBuildersAmount.ToString());
            _buildTime.SetText(data.BuildTime.ToString());
            
            _buildSystemCost.UpdateCost(data.Costs);

            StartCoroutine(RefreshContentSizeFitter());
        }

        private IEnumerator RefreshContentSizeFitter()
        {
            _buildingSystemInfoFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            yield return null;
            _buildingSystemInfoFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnInfoPanelHovered?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnInfoPanelExit?.Invoke();
        }
    }
}
