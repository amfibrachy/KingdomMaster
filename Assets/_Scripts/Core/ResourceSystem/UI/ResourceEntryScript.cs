namespace _Scripts.Core.ResourceSystem.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ResourceEntryScript : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;

        public void Set(Sprite icon, string amount)
        {
            _icon.sprite = icon;
            _amount.text = amount;
        }

        public void SetEnabled(bool status)
        {
            gameObject.SetActive(status);
        }
    }
}
