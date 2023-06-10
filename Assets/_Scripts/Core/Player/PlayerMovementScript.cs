namespace _Scripts.Core.Player
{
    using Animations;
    using UnityEngine;
    using Utils.Debugging;

    public class PlayerMovementScript : MonoBehaviour
    {
        [SerializeField] private AnimationControllerScript _playerAnimation;
        [SerializeField] private float _speed = 5f;

        private float _currentDirection;

        private void Update()
        {
            _currentDirection = Input.GetAxisRaw("Horizontal");

            if (_currentDirection > 0)
            {
                transform.localScale = new Vector3(1,1,1);
                _playerAnimation.SetState(_playerAnimation.Walk);

                CDebug.Log("Moving Right");
            }
            else if (_currentDirection < 0)
            {
                transform.localScale = new Vector3(-1,1,1);
                _playerAnimation.SetState(_playerAnimation.Walk);
                
                CDebug.Log("Moving Left");
            }
            else
            {
                _playerAnimation.SetState(_playerAnimation.Idle);
                CDebug.Log("Stopped moving");
            }
        }

        private void LateUpdate()
        {
            transform.Translate(_currentDirection * _speed * Time.deltaTime, 0, 0, Space.World);
        }
    }
}
