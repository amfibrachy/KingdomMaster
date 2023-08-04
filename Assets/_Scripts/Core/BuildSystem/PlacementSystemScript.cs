namespace _Scripts.Core.BuildSystem
{
    using System;
    using System.Globalization;
    using _Scripts.Utils.Debugging;
    using global::Zenject;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlacementSystemScript : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _buildingZoneMask;
        [SerializeField] private LayerMask _buildingsLayer;
        [SerializeField] private Material _transparentMaterialPrefab;
        [SerializeField] private Color _invalidColor;
        [SerializeField] private Color _tooFarColor;
        [SerializeField] private Color _validColor;
        
        [SerializeField] private float _transparencyValue = 0.5f;
        [SerializeField] private float _distanceToPlayer = 20f;
        [SerializeField] private float _rulerDisplayHeight = 5f;
        [SerializeField] private float _rulerShowMinDistance = 1f;
        
        [Header("References")] 
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _rulerTransform;
        [SerializeField] private SpriteRenderer _rulerRenderer;
        [SerializeField] private TextMeshPro _distanceText;

        public event Action<BuildingDataSO, Vector2> OnBuildingPlaced;
        
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
        
        // Privates
        private BuildingDataSO _buildingData;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        private enum ValidityState
        {
            None,
            Valid,
            Invalid,
            FarPlayer,
            FarLeft,
            FarRight,
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
                _raycastHit = Physics2D.GetRayIntersection(ray, 100f, _buildingZoneMask);
                
                if (_raycastHit.collider)
                {
                    _toBuild.Activate();

                    _toBuild.transform.position = new Vector3(Mathf.RoundToInt(_raycastHit.point.x), 0f);
                    var positionValidityState = IsPlacementValid();
                    
                    if (_previousValidityState != positionValidityState)
                    {
                        switch (positionValidityState)
                        {
                            case ValidityState.Valid:
                                _transparentMaterial.SetColor(Color1, _validColor);
                                _transparentMaterial.SetFloat(Alpha, _transparencyValue);

                                break;
                            
                            case ValidityState.Invalid:
                                _transparentMaterial.SetColor(Color1, _invalidColor);
                                _transparentMaterial.SetFloat(Alpha, _transparencyValue);
                                
                                break;
                            
                            case ValidityState.FarLeft:
                            case ValidityState.FarRight:
                            case ValidityState.FarPlayer:
                                _transparentMaterial.SetColor(Color1, _tooFarColor);
                                _transparentMaterial.SetFloat(Alpha, _transparencyValue);
                                
                                break;
                            
                            case ValidityState.None:
                                break;
                            
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _toBuild.SetMaterial(_transparentMaterial);
                        _previousValidityState = positionValidityState;
                        SetRulerEnabled(false);
                    }
                    
                    if (_mouseAction.triggered && positionValidityState == ValidityState.Valid)
                    {
                        StartBuilding(_toBuild.transform.position);
                    }
                }
                else
                {
                    _toBuild.Deactivate();
                    SetRulerEnabled(false);
                }
            }
        }
        
        public void StartPlacement(BuildingDataSO buildingData)
        {
            _buildingData = buildingData;
            StopPlacement();
            
            _toBuild = Instantiate(buildingData.Prefab);
            _toBuild.OnCollisionEnter += OnPlacementInvalid;
            _toBuild.OnCollisionExit += OnPlacementValid;
            _toBuild.Initialize(buildingData);

            SetRulerEnabled(false);
            _activeCollisions = 0;
            _toBuild.Deactivate();

            InputManager.UI.Enable();
            _placementActive = true;
        }
        
        private void StartBuilding(Vector2 position)
        {
            StopPlacement();
            OnBuildingPlaced?.Invoke(_buildingData, position);
        }

        public void StopPlacement()
        {
            _placementActive = false;
            SetRulerEnabled(false);
            
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
            if (_activeCollisions != 0)
            {
                // SetRulerEnabled(false);
                return ValidityState.Invalid;
            }

            if (CheckPlayerDistance() == false)
            {
                // SetRulerEnabled(false);
                return ValidityState.FarPlayer;
            }

            if (CheckBuildingDistance(Vector2.left) == false)
            {
                return ValidityState.FarLeft;
            }
            
            if (CheckBuildingDistance(Vector2.right) == false)
            {
                return ValidityState.FarRight;
            }

            return ValidityState.Valid;
        }

        private bool CheckPlayerDistance()
        {
            var currentPosition = _toBuild.transform.position;
            float distanceToPlayer = Vector2.Distance(currentPosition, _player.transform.position);
            var isPlayerNear = distanceToPlayer <= _distanceToPlayer;
            return isPlayerNear;
        }

        private bool CheckBuildingDistance(Vector2 direction)
        {
            var currentPosition = _toBuild.transform.position;
            var raycastHits = Physics2D.RaycastAll(currentPosition, direction, _buildingData.MinBuildDistance, _buildingsLayer);

            // Start from index 1 to ignore self collision
            for (var index = 1; index < raycastHits.Length; index++)
            {
                var hit = raycastHits[index];
                var building = hit.collider.GetComponent<BuildingPlacementScript>();
                var colliderSizeWidth = hit.collider.GetComponent<BoxCollider2D>().size.x;

                if (building != null && building.Type == _buildingData.Type)
                {
                    float distanceToBuilding = Vector2.Distance(currentPosition, building.transform.position);
                    var exactDistance = distanceToBuilding - colliderSizeWidth;
                    
                    var isBuildingNear = distanceToBuilding <= _buildingData.MinBuildDistance;

                    if (isBuildingNear)
                    {
                        if (exactDistance < _rulerShowMinDistance)
                        {
                            SetRulerEnabled(false);
                        }
                        else
                        {
                            var rulerPosition = (currentPosition + building.transform.position) / 2f;
                            transform.position = new Vector3(rulerPosition.x, rulerPosition.y + _rulerDisplayHeight, rulerPosition.z);

                            _rulerRenderer.size = new Vector2(Mathf.Max(1f, exactDistance), 0.52f);
                            _distanceText.text = Mathf.RoundToInt(exactDistance).ToString(CultureInfo.InvariantCulture);
                            SetRulerEnabled(true);
                        }
                    }
                    else
                    {
                        SetRulerEnabled(false);
                    }

                    return !isBuildingNear;
                }
            }
            
            return true;
        }

        private void SetRulerEnabled(bool status)
        {
            _rulerTransform.gameObject.SetActive(status);
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
