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
        private ValidityState _isValidPos;

        private bool _placementActive;
        
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
                    
                    if (_previousValidityState != _isValidPos)
                    {
                        if (_isValidPos == ValidityState.Valid)
                        {
                            _transparentMaterial.SetColor(Color1, _validColor);
                            _transparentMaterial.SetFloat(Alpha, _transparencyValue);

                            _toBuild.SetMaterial(_transparentMaterial);
                        }
                        else if (_isValidPos == ValidityState.Invalid)
                        {
                            _transparentMaterial.SetColor(Color1, _invalidColor);
                            _transparentMaterial.SetFloat(Alpha, _transparencyValue);
                            
                            _toBuild.SetMaterial(_transparentMaterial);
                        }
                        
                        _previousValidityState = _isValidPos;
                    }
                    
                    if (_mouseAction.triggered && _isValidPos == ValidityState.Valid)
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
            _toBuild.OnCollisionEnter += PlacementInvalid;
            _toBuild.OnCollisionExit += PlacementValid;
            _toBuild.IsBuilt = false;
            _toBuild.Deactivate();

            InputManager.UI.Enable();
            _placementActive = true;
        }
        
        private void PlaceBuilding()
        {
            _toBuild.SetMaterial(_transparentMaterialPrefab);
            _toBuild.IsBuilt = true;
            _toBuild.OnCollisionEnter -= PlacementInvalid;
            _toBuild.OnCollisionExit -= PlacementValid;
            _toBuild = null;

            StopPlacement();
        }

        private void StopPlacement()
        {
            _placementActive = false;
            _previousValidityState = ValidityState.None;
            _isValidPos = ValidityState.Valid;

            if (_toBuild != null)
            {
                _toBuild.OnCollisionEnter -= PlacementInvalid;
                _toBuild.OnCollisionExit -= PlacementValid;
                
                Destroy(_toBuild.gameObject);
            }
            
            InputManager.UI.Disable();
        }

        private void PlacementValid()
        {
            _isValidPos = ValidityState.Valid;
        }
        
        private void PlacementInvalid()
        {
            _isValidPos = ValidityState.Invalid;
        }
        
        private void OnCancel(InputAction.CallbackContext context)
        {
            StopPlacement();
        }
    }
}
