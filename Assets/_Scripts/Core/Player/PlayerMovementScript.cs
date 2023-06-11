namespace _Scripts.Core.Player
{
    using System;
    using Animations;
    using InputActions;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Utils.Debugging;

    public class PlayerMovementScript : MonoBehaviour
    {
        [SerializeField] private AnimationControllerScript _playerAnimation;
        [SerializeField] private float _speed = 5f;

        private float _currentDirection;
        
        // Input System
        private InputActions _input;
        private InputAction _moveAction;
        
        private void Awake()
        {
            _input = new InputActions();
            _moveAction = _input.Player.Move;
  
            SubscribeListeners();
        }

        private void OnEnable()
        {
            _moveAction.Enable();
        }

        public void OnDisable()
        {
            _moveAction.Disable();
        }

        private void SubscribeListeners()
        {
        }

        private void Update()
        {
            _currentDirection = _moveAction.ReadValue<float>();

            if (_currentDirection > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                _playerAnimation.SetState(_playerAnimation.Walk);

                CDebug.Log("Moving Right: " + _currentDirection);
            }
            else if (_currentDirection < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                _playerAnimation.SetState(_playerAnimation.Walk);

                CDebug.Log("Moving Left: " + _currentDirection);
            }
            else
            {
                _playerAnimation.SetState(_playerAnimation.Idle);
                CDebug.Log("Stopped moving: " + _currentDirection);
            }
            
            transform.Translate(_currentDirection * _speed * Time.deltaTime, 0, 0, Space.World);
        }
    }
}
