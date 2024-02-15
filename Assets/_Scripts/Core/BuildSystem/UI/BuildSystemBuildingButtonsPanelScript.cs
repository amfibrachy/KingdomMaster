namespace _Scripts.Core.BuildSystem.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class BuildSystemBuildingButtonsPanelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnBuildingPanelHovered;
        public event Action OnBuildingPanelExit;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnBuildingPanelHovered?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnBuildingPanelExit?.Invoke();
        }
    }
}
