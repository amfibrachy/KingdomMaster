namespace _Scripts.Core.BuildSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class BuildingPlacementScript : MonoBehaviour
    {
        public event Action OnCollisionEnter;
        public event Action OnCollisionExit;
        
        public BuildingType Type => _data.Type;
        public BuildingDataSO Data => _data;

        [SerializeField] private BuildingDataSO _defaultSettings;
        
        [Header("Preview Settings")]
        [SerializeField] private string _previewSortingLayer;
        [SerializeField] private Transform _backgroundPreview;
        [SerializeField] private SpriteRenderer _backgroundRenderer;

        // Privates
        private TilemapRenderer[] _tileMapRenderers;
        private BuildingDataSO _data;
        
        private void Awake()
        {
            _tileMapRenderers = GetComponentsInChildren<TilemapRenderer>();
            _data = _defaultSettings;
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
            _data = data;
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
            var building = col.GetComponent<BuildingPlacementScript>();
            if (building != null)
            {
                OnCollisionEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var building = other.GetComponent<BuildingPlacementScript>();
            if (building != null)
            {
                OnCollisionExit?.Invoke();
            }
        }
    }
}
