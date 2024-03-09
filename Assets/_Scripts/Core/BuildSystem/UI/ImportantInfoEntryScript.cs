namespace _Scripts.Core.BuildSystem.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ImportantInfoEntryScript : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amount;

        public ImportantInfoType InfoType { get; private set; } 
        
        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }
        
        public void SetText(string text)
        {
            _text.SetText(text);
        }
        
        public void SetAmount(int amount)
        {
            _amount.SetText(amount.ToString());
        }

        public void SetType(ImportantInfoType type)
        {
            InfoType = type;
        }
    }
}
