namespace _Scripts.Core.BuildSystem.UI
{
    using System;
    using global::Zenject;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using Utils.Debugging;

    [RequireComponent(typeof(Image), typeof(Button))]
    public class BuildingEntryScript : MonoBehaviour, IPointerEnterHandler
    {
        // Injectables
        [Inject] private IDebug _debug;

        public event Action<BuildingDataSO> OnBuildingEntryHovered;
        public event Action<BuildingDataSO> OnBuildingEntryClicked;
        
        [SerializeField] private BuildingDataSO _data;
        [SerializeField] private Button _button;
        
        private void Awake()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnBuildingEntryClicked?.Invoke(_data);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnBuildingEntryHovered?.Invoke(_data);
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
