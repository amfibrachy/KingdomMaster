namespace _Scripts.Core.ResourceSystem
{
    using System;
    using DG.Tweening;
    using global::Zenject;
    using UnityEngine;
    using Utils.Debugging;

    public class TreeScript : MonoBehaviour
    {
        [Header("Tree details")]
        [SerializeField] private float _treeWidth = 1f;
        [SerializeField] private float _treeDurability = 100f;
        
        [Header("Effects")] 
        [SerializeField] private ParticleSystem _leaveParticles;
        [SerializeField] private GameObject _trunkPrefab;
        [SerializeField] private GameObject _fallenTreePrefab;
        
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
            transform.DOShakeRotation(0.4f, 10f, 7, 15f);
            _leaveParticles.Play();
            
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
            var position = transform.position;
            var trunk = Instantiate(_trunkPrefab, position, Quaternion.identity);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(transform.DOLocalMoveX( position.x + 0.15f, 0.5f).SetEase(Ease.OutQuart)).SetDelay(0.25f);
            sequence.Join(transform.DOLocalRotate(new Vector3(0, 0, -85), 1f).SetEase(Ease.InCirc));
            sequence.OnComplete(() =>
            {
                var fallenTree = Instantiate(_fallenTreePrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            });
        }
    }
}
