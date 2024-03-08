namespace _Scripts.Core.Player
{
    using AI;
    using Animations;
    using global::Zenject;
    using States;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerFSM : FSM<PlayerFSM>
    {
        [SerializeField] private AnimationControllerScript _dustAnimationController;
        
        // Injectables
        private Camera _mainCamera;
        
        // Input System
        private InputAction _moveAction;
        private InputAction _attackAction;
        
        /*************************************** Public Access To Different States and Objects  *******************************************/
        
        public PlayerMoveState MoveState;
        public PlayerAttackState AttackState;
        public InputAction MoveAction => _moveAction;
        public InputAction AttackAction => _attackAction;

        /************************************************************* Fields  *************************************************************/

        public float CurrentSpeed {get; private set; }
        public bool IsPlayerRunning { get; private set; }
        
        /************************************************************* Readonly Fields  *************************************************************/
        public Camera MainCamera => _mainCamera;
        public AnimationControllerScript DustAnimationController => _dustAnimationController;
        
        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void Awake()
        {
            CurrentSpeed = ((PlayerStats) Stats).WalkSpeed;
            IsPlayerRunning = false;
            
            InitInput(); 
        }

        public override void InitStates()
        {
            Agent = AgentType.Player;
            MoveState = new PlayerMoveState(this);
            AttackState = new PlayerAttackState(this);
            
            _currentState = MoveState;
            _currentState.EnterState();
        }

        public override void ShowParticles()
        {
        }

        private void InitInput()
        {
            InputManager.Player.Enable();
            _moveAction = InputManager.Player.Move;
            _attackAction = InputManager.Player.Attack;

            InputManager.Player.Run.performed += OnRunPerformed;
            InputManager.Player.Run.canceled += OnRunCanceled;
        }

        private void OnRunPerformed(InputAction.CallbackContext context)
        {
            if (_currentState == MoveState)
                SetRunEnabled(true);
        }
        
        private void OnRunCanceled(InputAction.CallbackContext context)
        {
            if (_currentState == MoveState)
                SetRunEnabled(false);
        }

        public Direction GetMoveDirection()
        {
            var currentDirection = MoveAction.ReadValue<float>();
            Direction direction = Direction.None;

            if (currentDirection > 0)
                direction = Direction.Right;
            else if (currentDirection < 0)
                direction = Direction.Left;
            
            return direction;
        }
        
        public Direction GetFacingDirection()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var screenMiddlePoint = MainCamera.scaledPixelWidth / 2;

            return mousePosition.x < screenMiddlePoint ? Direction.Left : Direction.Right;
        }

        public void SetRunEnabled(bool status)
        {
            IsPlayerRunning = status;

            if (IsPlayerRunning)
            {
                CurrentSpeed = ((PlayerStats) Stats).RunSpeed;
            }
            else
            {
                DustAnimationController.gameObject.SetActive(false);
                
                CurrentSpeed = ((PlayerStats) Stats).WalkSpeed;
            }
        }
    }
}
