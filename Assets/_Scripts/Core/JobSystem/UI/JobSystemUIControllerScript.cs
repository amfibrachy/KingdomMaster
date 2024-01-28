namespace _Scripts.Core.JobSystem.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using DG.Tweening;
    using global::Zenject;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class JobSystemUIControllerScript : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _jobsSystemCanvasGroup;
        [SerializeField] private RectTransform _jobsPanelTransform;
        [SerializeField] private float _outsidePosX = 455f;
        
        [Header("Sluggards")] 
        [SerializeField] private TextMeshProUGUI _sluggardsNumberText;
        [SerializeField] private TextMeshProUGUI _sluggardsNumberDecreaseText;

        [Header("Buttons")] 
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _discardButton;
        [SerializeField] private Button _exitButton;
        
        [Header("Sprites and Images")] 
        [SerializeField] private Image _applyButtonImage;
        [SerializeField] private Image _discardButtonImage;
        [SerializeField] private Sprite _blockedApplyButtonSprite;
        [SerializeField] private Sprite _blockedDiscardButtonSprite;
        
        [SerializeField] private JobEntryScript[] _jobEntries;
        
        public bool IsShown { private set; get; }

        public int SluggardsRequestCount { private set; get; }

        // Privates
        private SluggardsManager _sluggardsManager;
        private Dictionary<JobType, int> _jobRequests;
        private int TotalIncreaseCount => _jobRequests.Count > 0 ? _jobRequests.Values.Sum() : 0;
        
        private Sprite _applyButtonSprite;
        private Sprite _discardButtonSprite;
        
        [Inject]
        public void Construct(SluggardsManager sluggardsManager)
        {
            _sluggardsManager = sluggardsManager;
        }
        
        private void Awake()
        {
            _applyButton.onClick.AddListener(OnApplyClicked);
            _discardButton.onClick.AddListener(OnDiscardClicked);
            _exitButton.onClick.AddListener(OnExitClicked);

            _sluggardsManager.OnAvailableSluggardsChanged += UpdateButtonsValidity;

            foreach (var jobEntry in _jobEntries)
            {
                jobEntry.OnIncreaseClicked += JobEntryIncreaseClicked;
                jobEntry.OnDecreaseClicked += JobEntryDecreaseClicked;
            }

            _applyButtonSprite = _applyButtonImage.sprite;
            _discardButtonSprite = _discardButtonImage.sprite;

            SluggardsRequestCount = 0;
            _jobRequests = new Dictionary<JobType, int>();
        }
        
        private void OnDestroy()
        {
            _applyButton.onClick.RemoveListener(OnApplyClicked);
            _discardButton.onClick.RemoveListener(OnDiscardClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
            
            _sluggardsManager.OnAvailableSluggardsChanged -= UpdateSluggardsNumberText;
        }
        
        public void ShowPanel()
        {
            DiscardChanges();
            UpdateSluggardsNumberText();

            _jobsSystemCanvasGroup.DOFade(1f, 0.15f);
            _jobsPanelTransform.DOAnchorPosX(0f, 0.3f).OnComplete(() =>
            {
                IsShown = true;
            });
        }

        private void UpdateSluggardsNumberText()
        {
            _sluggardsNumberText.text = _sluggardsManager.SluggardCount.ToString();
        }
        
        public void HidePanel()
        {
            DiscardChanges();
            
            _jobsSystemCanvasGroup.DOFade(0f, 0.15f);
            _jobsPanelTransform.DOAnchorPosX(_outsidePosX, 0.3f).OnComplete(() =>
            {
                IsShown = false;
            });
        }

        private void SetSluggardDecreaseTextEnabled(bool status)
        {
            _sluggardsNumberDecreaseText.gameObject.SetActive(status);
        }
        
        private void UpdateSluggardDecreaseText(int amount = 0)
        {
            int total = SluggardsRequestCount + amount;
            _sluggardsNumberDecreaseText.text = "-" + total;
        }
        
        public void BlockApplyButton()
        {
            _applyButton.interactable = false;
            _applyButtonImage.sprite = _blockedApplyButtonSprite;
        }
        
        public void BlockDiscardButton()
        {
            _discardButton.interactable = false;
            _discardButtonImage.sprite = _blockedDiscardButtonSprite;
        }
        
        public void UnblockApplyButton()
        {
            _applyButton.interactable = true;
            _applyButtonImage.sprite = _applyButtonSprite;
        }
        
        public void UnblockDiscardButton()
        {
            _discardButton.interactable = true;
            _discardButtonImage.sprite = _discardButtonSprite;
        }
        
        private void DiscardChanges()
        {
            ClearJobs();
                
            foreach (var jobEntry in _jobEntries)
            {
                jobEntry.Discard();
                
                if (_sluggardsManager.SluggardCount > 0)
                {
                    jobEntry.UnblockIncreaseButton();
                }
            }
            
            BlockApplyButton();
            BlockDiscardButton();

            if (SluggardsRequestCount > 0)
            {
                SetSluggardDecreaseTextEnabled(true);
                UpdateSluggardDecreaseText();
            }
            else
            {
                SetSluggardDecreaseTextEnabled(false);
            }
        }

        private void UpdateButtonsValidity()
        {
            if (IsShown)
            {
                UpdateSluggardsNumberText();
                bool insufficientSluggards = TotalIncreaseCount > _sluggardsManager.SluggardCount;

                if (insufficientSluggards)
                {
                    BlockApplyButton();
                }
                
                foreach (var jobEntry in _jobEntries)
                {
                    if (insufficientSluggards)
                    {
                        jobEntry.BlockIncreaseButton();
                        jobEntry.SetIncreaseTextWarning(true);
                    }
                    else
                    {
                        jobEntry.SetIncreaseTextWarning(false);
                    }
                }
            }
        }

        private void OnDiscardClicked()
        {
            DiscardChanges();
        }

        private void OnExitClicked()
        {
            HidePanel();
        }

        private void OnApplyClicked()
        {
            foreach (var jobEntry in _jobEntries)
            {
                jobEntry.Apply();
            }

            SluggardsRequestCount += TotalIncreaseCount;
            _sluggardsManager.CreateJobRequests(_jobRequests);
            
            DiscardChanges();
        }

        private void AddJob(JobType type)
        {
            if (_jobRequests.ContainsKey(type))
            {
                _jobRequests[type]++;
            }
            else
            {
                _jobRequests.Add(type, 1);
            }
        }
        
        private void RemoveJob(JobType type)
        {
            if (_jobRequests.ContainsKey(type))
            {
                if (_jobRequests[type] == 1)
                {
                    _jobRequests.Remove(type);
                }
                else
                {
                    _jobRequests[type]--;
                }
            }
        }

        private void ClearJobs()
        {
            _jobRequests.Clear();
        }
        
        private void JobEntryIncreaseClicked(JobType type)
        {
            if (TotalIncreaseCount <= _sluggardsManager.SluggardCount)
            {
                AddJob(type);

                UpdateSluggardDecreaseText(TotalIncreaseCount);
                SetSluggardDecreaseTextEnabled(true);
                UnblockApplyButton();
                UnblockDiscardButton();

                if (TotalIncreaseCount == _sluggardsManager.SluggardCount)
                {
                    foreach (var jobEntry in _jobEntries)
                    {
                        jobEntry.BlockIncreaseButton();
                    }
                }
            }
        }
        
        private void JobEntryDecreaseClicked(JobType type)
        {
            if (TotalIncreaseCount > 0)
            {
                RemoveJob(type);
                UpdateSluggardDecreaseText(TotalIncreaseCount);
                
                foreach (var jobEntry in _jobEntries)
                {
                    jobEntry.UnblockIncreaseButton();
                }
                
                if (TotalIncreaseCount == 0)
                {
                    BlockApplyButton();
                    BlockDiscardButton();
                    SetSluggardDecreaseTextEnabled(SluggardsRequestCount > 0);
                }
            }
        }
    }
}
