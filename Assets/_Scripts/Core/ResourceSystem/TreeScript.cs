namespace _Scripts.Core.ResourceSystem
{
    using System;
    using DG.Tweening;
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;

    public class TreeScript : MonoBehaviour
    {
        [SerializeField] private float _treeWidth = 1f;
        [SerializeField] private float _treeDurability = 100f;

        [Inject] private IDebug _debug;

        public event Action<TreeScript> OnTreeChopped;
        
        public float TreeWidth => _treeWidth;
        public bool IsMarked { get; private set; }
        public bool IsChoppedDown { get; private set; }
        
        // Privates
        private float _currentProgress;
        
        public void MarkToCut()
        {
            IsMarked = true;
        }
        
        public void ChopTree(float amount)
        {
            _treeDurability -= amount;
            transform.DOShakeRotation(0.5f, 4f, 10, 10f);

            _debug.Log("_treeDurability: " + _treeDurability);
            
            if (_treeDurability <= 0)
            {
                _treeDurability = 0;
                IsChoppedDown = true;
                OnTreeChopped?.Invoke(this);
                FallTree();
            }
        }

        private void FallTree()
        {
            _debug.Log("Tree fell!");
        }
    }
}
