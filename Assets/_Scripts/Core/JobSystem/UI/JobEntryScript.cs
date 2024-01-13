namespace _Scripts.Core.JobSystem.UI
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class JobEntryScript : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _quantityText;
        [SerializeField] private TextMeshPro _requestText;
        [SerializeField] private Button _increaseButton;
        [SerializeField] private Button _decreaseButton;

        private void Awake()
        {
            _increaseButton.onClick.AddListener(OnIncreaseClicked);
            _decreaseButton.onClick.AddListener(OnDecreaseClicked);
        }

        private void OnDestroy()
        {
            _increaseButton.onClick.RemoveListener(OnIncreaseClicked);
            _decreaseButton.onClick.RemoveListener(OnDecreaseClicked);
        }

        private void OnIncreaseClicked()
        {
            
        }
        
        private void OnDecreaseClicked()
        {
            
        }
    }
}
