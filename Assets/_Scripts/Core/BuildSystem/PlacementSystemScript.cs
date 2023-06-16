namespace _Scripts.Core.UI.BuildSystem
{
    using System;
    using global::Zenject;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using Utils.Debugging;

    public class PlacementSystemScript : MonoBehaviour
    {
        [SerializeField] private LayerMask _buildingMask;
        
        // Injectables
        private IDebug _debug;
        private Camera _camera;
        
        private GameObject _buildingPrefab;
        private GameObject _toBuild;
        
        private RaycastHit2D _raycastHit;

        [Inject]
        public void Construct(Camera mainCamera, IDebug debug)
        {
            _debug = debug;
            _camera = mainCamera;
        }

        private void Awake()
        {
            _buildingPrefab = null;
        }

        private void Update()
        {
            if (_buildingPrefab != null)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                _raycastHit = Physics2D.GetRayIntersection(ray, 100f, _buildingMask);
                
                _debug.Log(_raycastHit.point);
                
                if (_raycastHit.collider)
                {
                    if (!_toBuild.activeSelf)
                        _toBuild.SetActive(true);

                    _toBuild.transform.position = new Vector3(Mathf.RoundToInt(_raycastHit.point.x), 0f);
                }
                else
                {
                    if (_toBuild.activeSelf) 
                        _toBuild.SetActive(false);
                }
            }
        }

        public void SetBuildingPrefab(GameObject prefab)
        {
            _buildingPrefab = prefab;
            CreateBuilding();
        }

        private void CreateBuilding()
        {
            if (_toBuild != null) 
                Destroy(_toBuild);
            
            _toBuild = Instantiate(_buildingPrefab);
            _toBuild.SetActive(false);
        }
    }
}
