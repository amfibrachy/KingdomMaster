namespace _Scripts.Core.JobSystem.UI
{
    using System.Collections.Generic;
    using System.Linq;
    using AI;
    using DG.Tweening;
    using global::Zenject;
    using NPC;
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
        [SerializeField] private Image _sluggardsInMotionIcon;

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

        // Injectables
        private SluggardsManager _sluggardsManager;
        private BuildersManager _buildersManager;
        
        // Privates
        private Dictionary<JobType, int> _jobRequests;
        private int TotalIncreaseCount => _jobRequests.Count > 0 ? _jobRequests.Values.Sum() : 0;
        
        private Sprite _applyButtonSprite;
        private Sprite _discardButtonSprite;
        
        [Inject]
        public void Construct(SluggardsManager sluggardsManager, BuildersManager buildersManager)
        {
            _sluggardsManager = sluggardsManager;
            _buildersManager = buildersManager;
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

        private void Start()
        {
            SetInitialJobCounts();
        }

        private void OnDestroy()
        {
            _applyButton.onClick.RemoveListener(OnApplyClicked);
            _discardButton.onClick.RemoveListener(OnDiscardClicked);
            _exitButton.onClick.RemoveListener(OnExitClicked);
            
            _sluggardsManager.OnAvailableSluggardsChanged -= UpdateButtonsValidity;
            
            foreach (var jobEntry in _jobEntries)
            {
                jobEntry.OnIncreaseClicked -= JobEntryIncreaseClicked;
                jobEntry.OnDecreaseClicked -= JobEntryDecreaseClicked;
            }
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

        public void HidePanel()
        {
            DiscardChanges();
            
            _jobsSystemCanvasGroup.DOFade(0f, 0.15f);
            _jobsPanelTransform.DOAnchorPosX(_outsidePosX, 0.3f).OnComplete(() =>
            {
                IsShown = false;
            });
        }

        public void UpdateJobUIOnCreate(JobType job)
        {
            foreach (var jobEntry in _jobEntries)
            {
                if (jobEntry.JobType != JobType.None && jobEntry.JobType == job)
                {
                    jobEntry.DecreaseRequestCount();
                    jobEntry.IncreaseJobCount();

                    SluggardsRequestCount--;
                    
                    if (SluggardsRequestCount + TotalIncreaseCount > 0)
                    {
                        SetSluggardsInMotionEnabled(true);
                        UpdateSluggardDecreaseText(TotalIncreaseCount);
                    }
                    else
                    {
                        SetSluggardsInMotionEnabled(false);
                    }
                    
                    break;
                }
            }
        }
        
        public void UpdateJobUIOnDispatch<T>(FSM<T> npc, JobType job) where T : IFSM<T>
        {
            foreach (var jobEntry in _jobEntries)
            {
                if (jobEntry.JobType != JobType.None && jobEntry.JobType == job)
                {
                    jobEntry.DecreaseJobCount();
                    
                    break;
                }
            }
        }
        
        private void UpdateSluggardsNumberText()
        {
            _sluggardsNumberText.text = _sluggardsManager.Count.ToString();
        }
        
        private void SetSluggardsInMotionEnabled(bool status)
        {
            _sluggardsNumberDecreaseText.gameObject.SetActive(status);
            _sluggardsInMotionIcon.gameObject.SetActive(status);
        }
        
        private void UpdateSluggardDecreaseText(int amount = 0)
        {
            int total = SluggardsRequestCount + amount;
            _sluggardsNumberDecreaseText.text = total.ToString();
        }

        private void BlockApplyButton()
        {
            _applyButton.interactable = false;
            _applyButtonImage.sprite = _blockedApplyButtonSprite;
        }

        private void BlockDiscardButton()
        {
            _discardButton.interactable = false;
            _discardButtonImage.sprite = _blockedDiscardButtonSprite;
        }

        private void UnblockApplyButton()
        {
            _applyButton.interactable = true;
            _applyButtonImage.sprite = _applyButtonSprite;
        }

        private void UnblockDiscardButton()
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
                
                if (_sluggardsManager.Count > 0)
                {
                    jobEntry.UnblockIncreaseButton();
                }
            }
            
            BlockApplyButton();
            BlockDiscardButton();

            if (SluggardsRequestCount > 0)
            {
                SetSluggardsInMotionEnabled(true);
                UpdateSluggardDecreaseText();
            }
            else
            {
                SetSluggardsInMotionEnabled(false);
            }
        }

        private void UpdateButtonsValidity()
        {
            if (IsShown)
            {
                UpdateSluggardsNumberText();
                bool insufficientSluggards = TotalIncreaseCount > _sluggardsManager.Count;

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

        private void SetInitialJobCounts()
        {
            foreach (var jobEntry in _jobEntries)
            {
                switch (jobEntry.JobType)
                {
                    case JobType.Builder:
                        jobEntry.SetJobCount(_buildersManager.Count);
                        break;
                    case JobType.Hauler:
                        break;
                    case JobType.Lumberjack:
                        break;
                    case JobType.Miner:
                        break;
                    case JobType.Farmer:
                        break;
                    case JobType.Blacksmith:
                        break;
                    case JobType.Cook:
                        break;
                    case JobType.Fisherman:
                        break;
                    case JobType.Herbalist:
                        break;
                    case JobType.Alchemist:
                        break;
                    case JobType.Engineer:
                        break;
                    case JobType.None:
                        break;
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
            if (TotalIncreaseCount <= _sluggardsManager.Count)
            {
                AddJob(type);

                UpdateSluggardDecreaseText(TotalIncreaseCount);
                SetSluggardsInMotionEnabled(true);
                UnblockApplyButton();
                UnblockDiscardButton();

                if (TotalIncreaseCount == _sluggardsManager.Count)
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
                    SetSluggardsInMotionEnabled(SluggardsRequestCount > 0);
                }
            }
        }
    }
}
