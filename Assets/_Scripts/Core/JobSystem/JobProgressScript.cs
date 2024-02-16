namespace _Scripts.Core.JobSystem
{
    using System;
    using BuildSystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class JobProgressScript : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _icon;

        public event Action OnCreated; 

        // Privates
        private float _currentTime;
        private float _goalTime;
        private bool _isCreating;

        private void Awake()
        {
            _slider.minValue = 0;
            _slider.value = 0;
        }

        public void Initialize(BuildingDataSO data)
        {
            _currentTime = 0;
            _goalTime = data.TrainingTime;
            _slider.maxValue = data.TrainingTime;
            _icon.sprite = data.JobSprite;
        }
        
        public void StartCreation()
        {
            _isCreating = true;
        }

        public void PauseCreation()
        {
            _isCreating = false;
        }

        private void Update()
        {
            if (_isCreating)
            {
                _currentTime += Time.deltaTime;
                _slider.value = _currentTime;

                if (_currentTime >= _goalTime)
                {
                    _slider.value = _goalTime;
                    PauseCreation();
                    OnCreated?.Invoke();
                }
            }
        }
    }
}
