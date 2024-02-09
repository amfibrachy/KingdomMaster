namespace _Scripts.Core.JobSystem.UI
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class JobEntryScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _peopleText;
        [SerializeField] private TextMeshProUGUI _requestText;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private Button _decreaseButton;

        [Header("Sprites and Images")] 
        [SerializeField] private Image _increaseButtonImage;
        [SerializeField] private Image _decreaseButtonImage;
        [SerializeField] private Sprite _blockedIncreaseButtonSprite;
        [SerializeField] private Sprite _blockedDecreaseButtonSprite;
        [SerializeField] private Image _increaseFrameImage;
        [SerializeField] private Image _decreaseFrameImage;
        
        [SerializeField] private JobType _jobType;

        public JobType JobType => _jobType;
        public int PeopleCount { private set; get; }
        public int IncreaseCount { private set; get; }
        public int RequestCount { private set; get; }
        
        public event Action<JobType> OnIncreaseClicked; 
        public event Action<JobType> OnDecreaseClicked;

        private Sprite _increaseButtonSprite;
        private Sprite _decreaseButtonSprite;
        
        private void Awake()
        {
            _increaseButton.onClick.AddListener(OnIncreaseButtonClicked);
            _decreaseButton.onClick.AddListener(OnDecreaseButtonClicked);

            _increaseButtonSprite = _increaseButtonImage.sprite;
            _decreaseButtonSprite = _decreaseButtonImage.sprite;

            IncreaseCount = 0;
            RequestCount = 0;
        }
        
        public void BlockIncreaseButton()
        {
            _increaseButton.interactable = false;
            _increaseButtonImage.sprite = _blockedIncreaseButtonSprite;
            _increaseFrameImage.enabled = false;
        }

        public void BlockDecreaseButton()
        {
            _decreaseButton.interactable = false;
            _decreaseButtonImage.sprite = _blockedDecreaseButtonSprite;
            _decreaseFrameImage.enabled = false;
        }
        
        public void UnblockIncreaseButton()
        {
            _increaseButton.interactable = true;
            _increaseButtonImage.sprite = _increaseButtonSprite;
            _increaseFrameImage.enabled = true;
        }
        
        public void UnblockDecreaseButton()
        {
            _decreaseButton.interactable = true;
            _decreaseButtonImage.sprite = _decreaseButtonSprite;
            _decreaseFrameImage.enabled = true;
        }

        public void Discard()
        {
            IncreaseCount = 0;
            BlockDecreaseButton();
            BlockIncreaseButton();

            if (RequestCount > 0)
            {
                UpdateRequestText();
                SetRequestTextEnabled(true);
            }
            else
            {
                SetRequestTextEnabled(false);
            }
        }

        public void Apply()
        {
            RequestCount += IncreaseCount;
        }

        private void UpdateRequestText(int amount = 0)
        {
            int total = RequestCount + amount;
            _requestText.text = "+" + total;
        }

        public void SetIncreaseTextWarning(bool setWarning)
        {
            if (setWarning)
            {
                _requestText.color = Color.red;
            }
            else
            {
                _requestText.color = Color.green;
            }
        }
        
        private void OnIncreaseButtonClicked()
        {
            IncreaseCount++;
            UpdateRequestText(IncreaseCount);
            SetRequestTextEnabled(true);
            UnblockDecreaseButton();
            
            OnIncreaseClicked?.Invoke(_jobType);
        }

        public void SetRequestTextEnabled(bool status)
        {
            _requestText.gameObject.SetActive(status);
        }
        
        private void OnDecreaseButtonClicked()
        {
            if (IncreaseCount > 0)
            {
                IncreaseCount--;
                UnblockIncreaseButton();
                UpdateRequestText(IncreaseCount);
                
                if (IncreaseCount == 0)
                {
                    SetRequestTextEnabled(RequestCount > 0);
                    BlockDecreaseButton();
                }
                
                OnDecreaseClicked?.Invoke(_jobType);
            }
        }

        public void DecreaseRequestCount()
        {
            RequestCount--;
            
            if (RequestCount + IncreaseCount > 0)
            {
                UpdateRequestText(IncreaseCount);
                SetRequestTextEnabled(true);
            }
            else
            {
                SetRequestTextEnabled(false);
            }
        }
        
        private void OnDestroy()
        {
            _increaseButton.onClick.RemoveListener(OnIncreaseButtonClicked);
            _decreaseButton.onClick.RemoveListener(OnDecreaseButtonClicked);
        }
    }
}
