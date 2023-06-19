namespace _Scripts.Core.UI.BuildSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class BuildingPlacementScript : MonoBehaviour
    {
        public event Action OnCollisionEnter;
        public event Action OnCollisionExit;
        
        private TilemapRenderer[] _renderers;
        
        public bool IsBuilt { get; set; }
        
        private void Awake()
        {
            _renderers = GetComponentsInChildren<TilemapRenderer>();
            IsBuilt = true;
        }

        public void SetMaterial(Material material)
        {
            foreach (var tilemapRenderer in _renderers)
            {
                tilemapRenderer.material = material;
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
            var building = col.GetComponent<BuildingPlacementScript>();
            if (building != null && building.IsBuilt)
            {
                OnCollisionEnter?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var building = other.GetComponent<BuildingPlacementScript>();
            if (building != null && building.IsBuilt)
            {
                OnCollisionExit?.Invoke();
            }
        }
    }
}
