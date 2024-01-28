namespace _Scripts.Core.Player
{
    using AI;
    using global::Zenject;
    using States;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Utils.Debugging;

    public class PlayerFSM : FSM<PlayerFSM>
    {
        // Injectables
        private Camera _mainCamera;
        
        // Input System
        private InputAction _moveAction;
        private InputAction _attackAction;
        
        private float _currentDirection;
        private bool _attackPerformed;
        
        public InputAction MoveAction => _moveAction;
        public InputAction AttackAction => _attackAction;

        // Public Access To Different States
        public Camera MainCamera => _mainCamera;
        public float CurrentSpeed {get; private set; }

        public bool IsPlayerRunning { get; private set; }
        public bool IsInBuildMode { get;  set; }

        public PlayerMoveState MoveState;
        public PlayerAttackState AttackState;
        
        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        private void Awake()
        {
            CurrentSpeed = ((PlayerStats) Stats).WalkSpeed;
            
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

        private void InitInput()
        {
            InputManager.Player.Enable();
            _moveAction = InputManager.Player.Move;
            _attackAction = InputManager.Player.Attack;

            InputManager.Player.RunToggle.performed += OnRunToggleOnperformed;
        }

        private void OnRunToggleOnperformed(InputAction.CallbackContext context)
        {
            if (_currentState == MoveState)
                ToggleRun();
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

        public void ToggleRun()
        {
            IsPlayerRunning = !IsPlayerRunning;

            if (IsPlayerRunning)
            {
                CurrentSpeed = ((PlayerStats) Stats).RunSpeed;
            }
            else
            {
                CurrentSpeed = ((PlayerStats) Stats).WalkSpeed;
            }
        }
    }
}
