namespace _Scripts.Core.BuildSystem
{
    using _Scripts.Core.Player;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class BuildSystemUIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _buildSystemUICanvasGroup;
        [SerializeField] private PlayerFSM _playerFsm;
        [SerializeField] private PlacementSystemScript _placementSystem;

        private bool IsBuildMode { get; set; }

        private void Awake()
        {
            InputManager.Player.BuildToggle.performed += OnBuildTogglePerformed;
        }

        private void Start()
        {
            IsBuildMode = false;
            HideBuildUI();
        }

        private void OnBuildTogglePerformed(InputAction.CallbackContext context)
        {
            IsBuildMode = !IsBuildMode;
            _playerFsm.IsInBuildMode = IsBuildMode;

            if (IsBuildMode)
            {
                ShowBuildUI();
            }
            else
            {
                _placementSystem.StopPlacement();
                HideBuildUI();
            }
        }

        public void ShowBuildUI()
        {
            _buildSystemUICanvasGroup.alpha = 1;
            _buildSystemUICanvasGroup.interactable = true;
        }
        
        public void HideBuildUI()
        {
            _buildSystemUICanvasGroup.alpha = 0;
            _buildSystemUICanvasGroup.interactable = false;
        }
    }
}
