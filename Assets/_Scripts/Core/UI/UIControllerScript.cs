namespace _Scripts.Core.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIControllerScript : MonoBehaviour
    {
        [SerializeField] private Button _houseButton;

        public Action OnHouseButtonClicked;
        
        private void Awake()
        {
            _houseButton.onClick.AddListener(OnHouseButtonClick);
        }

        private void OnHouseButtonClick()
        {
            OnHouseButtonClicked?.Invoke();
        }
    }
}
