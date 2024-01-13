namespace _Scripts.Core.BuildSystem.UI
{
    using System;
    using global::Zenject;
    using UnityEngine;
    using UnityEngine.UI;
    using Utils.Debugging;

    [RequireComponent(typeof(Image), typeof(Button))]
    public class BuildButtonScript : MonoBehaviour
    {
        // Injectables
        [Inject] private IDebug _debug;
        
        private Image _image;
        private Button _button;

        private int _id = -1;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
        }

        public void SetId(int id)
        {
            _id = id;
        }
        
        public void SetIcon(Sprite icon)
        {
            _image.sprite = icon;
        }
        
        public void SetOnClickCallback(Action<int> callback)
        {
            if (_id < 0)
            {
                _debug.LogError("Button id not set!");
            }
            
            _button.onClick.AddListener(() => callback.Invoke(_id));
        }
    }
}
