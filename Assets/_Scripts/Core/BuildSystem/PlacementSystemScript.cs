namespace _Scripts.Core.BuildSystem
{
    using System;
    using System.Globalization;
    using _Scripts.Utils.Debugging;
    using Global;
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
        
        [Header("Placement settings")]
        [SerializeField] private float _distanceToPlayer = 20f;
        [SerializeField] private float _rulerShowMinDistance = 1f;
        
        [Tooltip("1f = 32px, 0.5f = 16px, 0.25f = 8px, 0f = no snapping")]
        [SerializeField] private float _placementSnapping = 0.25f;
        
        [Header("References")] 
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _rulerTransform;
        [SerializeField] private SpriteRenderer _rulerRenderer;
        [SerializeField] private TextMeshPro _distanceText;

        public event Action<BuildingDataSO, Vector2> OnBuildingPlaced;
        
        // Injectables
        private IDebug _debug;
        private Camera _camera;
        private KingdomBordersController _kingdomBordersController;
        
        // Input System
        private InputAction _mouseAction;
        
        // Privates
        private BuildingPlacementScript _toBuild;
        private BoxCollider2D _toBuildCollider;
        private Material _transparentMaterial;

        private RaycastHit2D _raycastHit;
        private ValidityState _previousValidityState;

        private bool _placementActive;
        private int _activeCollisions;
        
        private BuildingDataSO _buildingData;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");

        private enum ValidityState
        {
            None,
            Valid,
            Invalid,
            FarFromPlayer,
            FarLeftFromBuilding,
            FarRightFromBuilding,
            FarFromBorder,
        }

        [Inject]
        public void Construct(Camera mainCamera, IDebug debug, KingdomBordersController kingdomBordersController)
        {
            _debug = debug;
            _camera = mainCamera;
            _kingdomBordersController = kingdomBordersController;
        }

        private void Start()
        {
            _transparentMaterial = new Material(_transparentMaterialPrefab);

            _mouseAction = InputManager.UI.Mouse;
            InputManager.UI.Cancel.performed += OnCancel;
        }
        
        private void Update()
        {
            if (!_placementActive)
                return;
            
            var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            _raycastHit = Physics2D.GetRayIntersection(ray, 100f, _buildingZoneMask);
            
            if (_raycastHit.collider)
            {
                _toBuild.Activate();

                _toBuild.transform.position = GetSnappedPosition(_raycastHit.point);
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
                        
                        case ValidityState.FarLeftFromBuilding:
                        case ValidityState.FarRightFromBuilding:
                        case ValidityState.FarFromBorder:
                        case ValidityState.FarFromPlayer:
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
                    PlaceConstructionSite(_toBuild.transform.position);
                }
            }
            else
            {
                _toBuild.Deactivate();
                SetRulerEnabled(false);
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
            _toBuildCollider = _toBuild.GetComponent<BoxCollider2D>();

            SetRulerEnabled(false);
            _activeCollisions = 0;
            _toBuild.Deactivate();

            InputManager.UI.Enable();
            _placementActive = true;
        }
        
        private void PlaceConstructionSite(Vector2 position)
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
                return ValidityState.Invalid;
            }

            if (CheckPlayerDistance() == false)
            {
                return ValidityState.FarFromPlayer;
            }

            if (CheckBuildingDistance(Vector2.left) == false)
            {
                return ValidityState.FarLeftFromBuilding;
            }
            
            if (CheckBuildingDistance(Vector2.right) == false)
            {
                return ValidityState.FarRightFromBuilding;
            }
            
            if (CheckBorderDistance() == false)
            {
                return ValidityState.FarFromBorder;
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
        
        private bool CheckBorderDistance()
        {
            var currentPositionX = _toBuild.transform.position.x;
            var townCenterPositionX = _kingdomBordersController.TownCenterPosition.x;
            var rightBorderPositionX = _kingdomBordersController.RightBorderPosition.x;
            var leftBorderPositionX = _kingdomBordersController.LeftBorderPosition.x;

            if (currentPositionX >= townCenterPositionX && currentPositionX >= rightBorderPositionX)
            {
                return false;
            }
            
            if (currentPositionX <= townCenterPositionX && currentPositionX <= leftBorderPositionX)
            {
                return false;
            }

            return true;
        }

        private bool CheckBuildingDistance(Vector2 direction)
        {
            var currentPosition = _toBuild.transform.position;
            var raycastOrigin = new Vector3(currentPosition.x + ((_toBuildCollider.bounds.extents.x + 0.1f) * direction.x), currentPosition.y, currentPosition.z);
            var raycastHits = Physics2D.RaycastAll(raycastOrigin, direction, _buildingData.MinBuildDistance, _buildingsLayer);

            // No need to start checking from index 1 because raycast origin happens on left/right extent + 0.1f offset
            for (var index = 0; index < raycastHits.Length; index++)
            {
                var hit = raycastHits[index];
                var building = hit.collider.GetComponent<BuildingPlacementScript>();

                if (building != null && building.Type == _buildingData.Type)
                {
                    float distanceToBuilding = Vector2.Distance(currentPosition, building.transform.position);
                    float exactDistance = distanceToBuilding - hit.collider.bounds.extents.x - _toBuildCollider.bounds.extents.x;

                    var isBuildingNear = exactDistance <= _buildingData.MinBuildDistance;

                    if (isBuildingNear)
                    {
                        if (exactDistance < _rulerShowMinDistance)
                        {
                            SetRulerEnabled(false);
                        }
                        else
                        {
                            var rulerPosition = (currentPosition + building.transform.position) / 2f;
                            transform.position = new Vector3(rulerPosition.x, rulerPosition.y + _buildingData.BuildingHeight, rulerPosition.z);

                            _rulerRenderer.size = new Vector2(Mathf.Max(1f, exactDistance), _rulerRenderer.size.y);
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

        private Vector3 GetSnappedPosition(Vector3 position)
        {
            return _placementSnapping > 0.01f
                ? new Vector3(Mathf.RoundToInt(position.x / _placementSnapping) * _placementSnapping, 0f) 
                : new Vector3(position.x, 0f);
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
