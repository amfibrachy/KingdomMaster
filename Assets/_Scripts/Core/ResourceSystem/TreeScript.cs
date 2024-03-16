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
        [SerializeField] private float _treeDisappearDelay = 1.5f;
        
        [Header("Effects")] 
        [SerializeField] private ParticleSystem _leaveParticles;
        [SerializeField] private Vector3 _leaveParticlesFallenPosition;
        [SerializeField] private ParticleSystem _treeDisappearParticles;
        [SerializeField] private GameObject _trunkPrefab;
        [SerializeField] private Transform _holder;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _fallenTreeSprite;
        
        [Inject] private IDebug _debug;

        public event Action<TreeScript> OnTreeChopped;
        public event Action OnResourcesAppeared;
        
        public float TreeWidth => _treeWidth;
        public bool IsMarked { get; private set; }
        public bool IsChoppedDown { get; private set; }
        
        // Privates
        private float _currentProgress;
        
        public void MarkToCut()
        {
            IsMarked = true;
        }
        
        public void UnMarkToCut()
        {
            IsMarked = false;
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

            sequence.Append(_holder.DOMoveX( position.x + 0.15f, 0.5f).SetEase(Ease.OutQuart)).SetDelay(0.25f);
            sequence.Join(_holder.DORotate(new Vector3(0, 0, -85), 1f).SetEase(Ease.InCirc));
            sequence.AppendCallback(() =>
            {
                _spriteRenderer.sprite = _fallenTreeSprite;
                _holder.rotation = Quaternion.identity;
                _leaveParticles.transform.localPosition = _leaveParticlesFallenPosition;
                _leaveParticles.Play();
            });
            sequence.AppendInterval(_treeDisappearDelay);
            sequence.OnComplete(() =>
            {
                _treeDisappearParticles.Play();
                _spriteRenderer.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    SpawnResources();
                    Destroy(gameObject, _treeDisappearParticles.totalTime);
                });
            });
        }

        private void SpawnResources()
        {
            OnResourcesAppeared?.Invoke();
        }
    }
}
