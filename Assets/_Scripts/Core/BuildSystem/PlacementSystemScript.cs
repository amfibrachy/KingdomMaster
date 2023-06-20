namespace _Scripts.Core.UI.BuildSystem
{
    using System;
    using System.Collections.Generic;
    using global::Zenject;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.Tilemaps;
    using Utils.Debugging;

    public class PlacementSystemScript : MonoBehaviour
    {
        [SerializeField] private LayerMask _buildingMask;
        [SerializeField] private Material _transparentMaterialPrefab;
        [SerializeField] private Color _invalidColor;
        [SerializeField] private Color _validColor;
        
        [SerializeField] private float _transparencyValue = 0.5f;
        
        // Injectables
        private IDebug _debug;
        private Camera _camera;
        
        private BuildingPlacementScript _toBuild;
        private Material _transparentMaterial;

        private RaycastHit2D _raycastHit;
        private ValidityState _previousValidityState;

        private bool _placementActive;
        private int _activeCollisions;
        
        // Input System
        private InputAction _mouseAction;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        private enum ValidityState
        {
            None,
            Valid,
            Invalid
        }

        [Inject]
        public void Construct(Camera mainCamera, IDebug debug)
        {
            _debug = debug;
            _camera = mainCamera;
        }

        private void Start()
        {
            _transparentMaterial = new Material(_transparentMaterialPrefab);

            _mouseAction = InputManager.UI.Mouse;
            InputManager.UI.Cancel.performed += OnCancel;
        }
        
        private void Update()
        {
            if (_placementActive)
            {
                var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
                _raycastHit = Physics2D.GetRayIntersection(ray, 100f, _buildingMask);
                
                if (_raycastHit.collider)
                {
                    _toBuild.Activate();

                    _toBuild.transform.position = new Vector3(Mathf.RoundToInt(_raycastHit.point.x), 0f);
                    var isPositionValid = IsPlacementValid();
                    
                    if (_previousValidityState != isPositionValid)
                    {
                        if (isPositionValid == ValidityState.Valid)
                        {
                            _transparentMaterial.SetColor(Color1, _validColor);
                            _transparentMaterial.SetFloat(Alpha, _transparencyValue);

                            _toBuild.SetMaterial(_transparentMaterial);
                        }
                        else if (isPositionValid == ValidityState.Invalid)
                        {
                            _transparentMaterial.SetColor(Color1, _invalidColor);
                            _transparentMaterial.SetFloat(Alpha, _transparencyValue);
                            
                            _toBuild.SetMaterial(_transparentMaterial);
                        }
                        
                        _previousValidityState = isPositionValid;
                    }
                    
                    if (_mouseAction.triggered && isPositionValid == ValidityState.Valid)
                    {
                        PlaceBuilding();
                    }
                }
                else
                {
                    _toBuild.Deactivate();
                }
            }
        }
        
        public void StartPlacement(BuildingPlacementScript prefab)
        {
            StopPlacement();
            
            _toBuild = Instantiate(prefab);
            _toBuild.OnCollisionEnter += OnPlacementInvalid;
            _toBuild.OnCollisionExit += OnPlacementValid;
            _toBuild.IsBuilt = false;
            _activeCollisions = 0;
            _toBuild.Deactivate();

            InputManager.UI.Enable();
            _placementActive = true;
        }
        
        private void PlaceBuilding()
        {
            _toBuild.SetMaterial(_transparentMaterialPrefab);
            _toBuild.IsBuilt = true;
            _toBuild.OnCollisionEnter -= OnPlacementInvalid;
            _toBuild.OnCollisionExit -= OnPlacementValid;
            _toBuild = null;

            StopPlacement();
        }

        private void StopPlacement()
        {
            _placementActive = false;
            _previousValidityState = ValidityState.None;

            if (_toBuild != null)
            {
                _toBuild.OnCollisionEnter -= OnPlacementInvalid;
                _toBuild.OnCollisionExit -= OnPlacementValid;
                
                Destroy(_toBuild.gameObject);
            }
            
            InputManager.UI.Disable();
        }

        private ValidityState IsPlacementValid()
        {
            return _activeCollisions == 0 ? ValidityState.Valid : ValidityState.Invalid;
        }

        private void OnPlacementValid()
        {
            _activeCollisions--;
        }
        
        private void OnPlacementInvalid()
        {
            _activeCollisions++;
        }
        
        private void OnCancel(InputAction.CallbackContext context)
        {
            StopPlacement();
        }
    }
}
