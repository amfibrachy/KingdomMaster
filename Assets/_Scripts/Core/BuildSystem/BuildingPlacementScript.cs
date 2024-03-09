namespace _Scripts.Core.BuildSystem
{
    using System;
    using UI;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class BuildingPlacementScript : MonoBehaviour
    {
        public event Action OnCollisionEnter;
        public event Action OnCollisionExit;

        [SerializeField] private BuildingImportantInfoController _importantInfoController;

        [Header("Preview Settings")]
        [SerializeField] private string _previewSortingLayer;
        [SerializeField] private Transform _backgroundPreview;
        [SerializeField] private SpriteRenderer _backgroundRenderer;

        // Privates
        private TilemapRenderer[] _tileMapRenderers;

        private void Awake()
        {
            _tileMapRenderers = GetComponentsInChildren<TilemapRenderer>();
        }

        public void SetMaterial(Material material)
        {
            foreach (var tilemapRenderer in _tileMapRenderers)
            {
                tilemapRenderer.material = material;
                tilemapRenderer.sortingLayerName = _previewSortingLayer;
            }

            _backgroundRenderer.material = material;
            _backgroundRenderer.sortingLayerName = _previewSortingLayer;
            _backgroundRenderer.sortingOrder = -1;
            _backgroundPreview.gameObject.SetActive(true);
        }

        public void Initialize(BuildingDataSO data)
        {
            _importantInfoController.InitializeImportantInfos(data);
        }

        public void UpdateImportantInfos(Vector3 position, bool isValid)
        {
            if (!_importantInfoController.HasImportantInfo) 
                return;
            
            if (isValid)
            {
                _importantInfoController.ShowInfos();
                _importantInfoController.UpdateImportantInfos(position);
            }
            else
            {
                _importantInfoController.HideInfos();
            }
        }

        public void Activate()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.layer == gameObject.layer)
            {
                OnCollisionEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == gameObject.layer)
            {
                OnCollisionExit?.Invoke();
            }
        }
    }
}
