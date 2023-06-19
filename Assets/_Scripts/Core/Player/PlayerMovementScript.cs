namespace _Scripts.Core.Player
{
    using System;
    using Animations;
    using global::Zenject;
    using InputActions;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Utils.Debugging;

    public class PlayerMovementScript : MonoBehaviour
    {
        [SerializeField] private AnimationControllerScript _playerAnimation;
        [SerializeField] private float _speed = 5f;

        private float _currentDirection;
        
        // Injectables
        private IDebug _debug;
        
        // Input System
        private InputAction _moveAction;
        
        private void Awake()
        {
            SubscribeListeners();
        }

        private void Start()
        {
            InputManager.Player.Enable();
            _moveAction = InputManager.Player.Move;
        }

        [Inject]
        public void Construct(IDebug debug)
        {
            _debug = debug;
        }

        private void SubscribeListeners()
        {
        }

        private void Update()
        {
            _currentDirection = _moveAction.ReadValue<float>();

            if (_currentDirection > 0)
            {
                _playerAnimation.TurnRight();
                _playerAnimation.SetState(_playerAnimation.Walk);
            }
            else if (_currentDirection < 0)
            {
                _playerAnimation.TurnLeft();
                _playerAnimation.SetState(_playerAnimation.Walk);
            }
            else
            {
                _playerAnimation.SetState(_playerAnimation.Idle);
            }
            
            transform.Translate(_currentDirection * _speed * Time.deltaTime, 0, 0, Space.World);
        }
    }
}
